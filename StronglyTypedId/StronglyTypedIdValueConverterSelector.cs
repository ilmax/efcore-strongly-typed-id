using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

namespace StronglyTypedId
{
    /// <summary>
    /// Based on https://andrewlock.net/strongly-typed-ids-in-ef-core-using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-4/
    /// </summary>
    public class StronglyTypedIdValueConverterSelector : ValueConverterSelector
    {
        private readonly ConcurrentDictionary<(Type ModelClrType, Type ProviderClrType), ValueConverterInfo> _converters
            = new ConcurrentDictionary<(Type ModelClrType, Type ProviderClrType), ValueConverterInfo>();

        public StronglyTypedIdValueConverterSelector(ValueConverterSelectorDependencies dependencies)
            : base(dependencies)
        { }

        public override IEnumerable<ValueConverterInfo> Select(Type modelClrType, Type providerClrType = null)
        {
            var baseConverters = base.Select(modelClrType, providerClrType);
            foreach (var converter in baseConverters)
            {
                yield return converter;
            }

            var underlyingModelType = UnwrapNullableType(modelClrType);
            var underlyingProviderType = UnwrapNullableType(providerClrType);

            if (underlyingProviderType is null || underlyingProviderType == typeof(long) || underlyingProviderType == typeof(int))
            {
                var isTypedIdValue = typeof(LongTypedIdValueBase).IsAssignableFrom(underlyingModelType);
                if (isTypedIdValue)
                {
                    yield return _converters.GetOrAdd((underlyingModelType, underlyingProviderType), _ =>
                    {
                        return new ValueConverterInfo(
                            modelClrType: modelClrType,
                            providerClrType: typeof(long),
                            factory: valueConverterInfo => (ValueConverter)Activator.CreateInstance(
                                typeof(TypedIdValueConverter<>).MakeGenericType(underlyingModelType)));
                    });
                }
            }
        }

        private static Type UnwrapNullableType(Type type)
        {
            if (type is null)
            {
                return null;
            }

            return Nullable.GetUnderlyingType(type) ?? type;
        }

        public class TypedIdValueConverter<TTypedIdValue> : ValueConverter<TTypedIdValue, long>
            where TTypedIdValue : LongTypedIdValueBase
        {
            public TypedIdValueConverter()
                : base(id => id.Value, value =>
                    Create(value), new ConverterMappingHints(valueGeneratorFactory: (p, t) => new TemporaryValueIdValueGenerator<TTypedIdValue>()))
            { }

            private static TTypedIdValue Create(long id) => Activator.CreateInstance(typeof(TTypedIdValue), id) as TTypedIdValue;
        }

        private class TemporaryValueIdValueGenerator<T> : ValueGenerator
        {
            private readonly TemporaryLongValueGenerator _innerGenerator = new TemporaryLongValueGenerator();

            protected override object NextValue(EntityEntry entry)
            {
                return Activator.CreateInstance(typeof(T), _innerGenerator.Next(entry));
            }

            public override bool GeneratesTemporaryValues => _innerGenerator.GeneratesTemporaryValues;
        }
    }
}