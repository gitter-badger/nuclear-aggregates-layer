﻿using System;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.UI.Metadata.Aspects;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.BL.UI.Metadata.Cards.Extensions
{
    public static class CardMetadataBuilderExtensions
    {
        public static CardMetadataBuilder<TEntity> WithDefaultMainAttribute<TEntity>(this CardMetadataBuilder<TEntity> builder)
            where TEntity : IEntityKey, IEntity
        {
            builder.WithFeatures(new MainAttributeFeature(new PropertyDescriptor<IIdentityAspect>(x => x.Id)));
            return builder;
        }
    }
}