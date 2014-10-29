﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

// ReSharper disable CheckNamespace

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
    // ReSharper restore CheckNamespace
{
    public partial class Client : IPartable
    {
        private IEnumerable<IEntityPart> _parts;

        public IEnumerable<IEntityPart> Parts
        {
            get { return _parts ?? (_parts = Enumerable.Empty<IEntityPart>()); }

            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException("Parts cannot be null");
                }

                _parts = value;
            }
        }
    }
}