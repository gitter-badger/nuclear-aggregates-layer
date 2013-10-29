//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// ReSharper disable RedundantUsingDirective
// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantNameQualifier
// ReSharper disable ConvertNullableToShortForm

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using DoubleGis.Erm.Model.Entities.Interfaces;
using DoubleGis.Erm.Model.Entities.Interfaces.Integration;
using DoubleGis.Erm.Model.Entities.ChangeTracking;


namespace DoubleGis.Erm.Model.Entities.Erm
{
    [System.CodeDom.Compiler.GeneratedCode("EF 4.0 STE generator", "1.0")]
    [DataContract(IsReference = true)]
    [KnownType(typeof(PerformedBusinessOperation))]
    
    
      public sealed partial class ServiceBusExportedBusinessOperation: 
    	  IObjectWithChangeTracker
    	, INotifyPropertyChanged
        
        , IEntityKey
        
        
        
        
    	  
        
        {
        #region Custom interfaces implementation
      
        #endregion

        #region Primitive Properties
    
        [DataMember]
        public int Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'Id' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    if (!IsDeserializing)
                    {
                        if (PerformedBusinessOperation != null && PerformedBusinessOperation.Id != value)
                        {
                            PerformedBusinessOperation = null;
                        }
                    }
                    _id = value;
                    OnPropertyChanged("Id");
                }
            }
        }
        private int _id;
          
        [DataMember]
        public bool Success
        {
            get { return _success; }
            set
            {
                if (_success != value)
                {
                    ChangeTracker.RecordOriginalValue("Success", _success);
                    _success = value;
                    OnPropertyChanged("Success");
                }
            }
        }
        private bool _success;
          
        [DataMember]
        public System.DateTime Date
        {
            get { return _date; }
            set
            {
                if (_date != value)
                {
                    ChangeTracker.RecordOriginalValue("Date", _date);
                    _date = value;
                    OnPropertyChanged("Date");
                }
            }
        }
        private System.DateTime _date;
          
        #endregion

        #region Navigation Properties
            [DataMember]
        public PerformedBusinessOperation PerformedBusinessOperation
        {
            get { return _performedBusinessOperation; }
            set
            {
                if (!ReferenceEquals(_performedBusinessOperation, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (Id != value.Id)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _performedBusinessOperation;
                    _performedBusinessOperation = value;
                    FixupPerformedBusinessOperation(previousValue);
                    OnNavigationPropertyChanged("PerformedBusinessOperation");
                }
            }
        }
        private PerformedBusinessOperation _performedBusinessOperation;

        #endregion

        #region ChangeTracking
    
        private void OnPropertyChanged(String propertyName)
        {
            if (ChangeTracker.State != ObjectState.Added && ChangeTracker.State != ObjectState.Deleted)
            {
                ChangeTracker.State = ObjectState.Modified;
            }
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    
        private void OnNavigationPropertyChanged(String propertyName)
        {
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged{ add { _propertyChanged += value; } remove { _propertyChanged -= value; } }
        private event PropertyChangedEventHandler _propertyChanged;
        private ObjectChangeTracker _changeTracker;
    
        [DataMember]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if (_changeTracker == null)
                {
                    _changeTracker = new ObjectChangeTracker();
                    _changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                }
                return _changeTracker;
            }
            set
            {
                if(_changeTracker != null)
                {
                    _changeTracker.ObjectStateChanging -= HandleObjectStateChanging;
                }
                _changeTracker = value;
                if(_changeTracker != null)
                {
                    _changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                }
            }
        }
    
        private void HandleObjectStateChanging(object sender, ObjectStateChangingEventArgs e)
        {
            if (e.NewState == ObjectState.Deleted)
            {
                ClearNavigationProperties();
            }
        }
    
        private bool IsDeserializing { get; set; }
    
        [OnDeserializing]
        public void OnDeserializingMethod(StreamingContext context)
        {
            IsDeserializing = true;
        }
    
        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            IsDeserializing = false;
            ChangeTracker.ChangeTrackingEnabled = true;
        }
    
        // This entity type is the dependent end in at least one association that performs cascade deletes.
        // This event handler will process notifications that occur when the principal end is deleted.
        internal void HandleCascadeDelete(object sender, ObjectStateChangingEventArgs e)
        {
            if (e.NewState == ObjectState.Deleted)
            {
                this.MarkAsDeleted();
            }
        }
    
        private void ClearNavigationProperties()
        {
            PerformedBusinessOperation = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupPerformedBusinessOperation(PerformedBusinessOperation previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && ReferenceEquals(previousValue.ServiceBusExportedBusinessOperation, this))
            {
                previousValue.ServiceBusExportedBusinessOperation = null;
            }
    
            if (PerformedBusinessOperation != null)
            {
                PerformedBusinessOperation.ServiceBusExportedBusinessOperation = this;
                Id = PerformedBusinessOperation.Id;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("PerformedBusinessOperation")
                    && (ChangeTracker.OriginalValues["PerformedBusinessOperation"] == PerformedBusinessOperation))
                {
                    ChangeTracker.OriginalValues.Remove("PerformedBusinessOperation");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("PerformedBusinessOperation", previousValue);
                }
                if (PerformedBusinessOperation != null && !PerformedBusinessOperation.ChangeTracker.ChangeTrackingEnabled)
                {
                    PerformedBusinessOperation.StartTracking();
                }
            }
        }

        #endregion

    }
}

// ReSharper enable RedundantUsingDirective
// ReSharper enable InconsistentNaming
// ReSharper enable PartialTypeWithSinglePart
// ReSharper enable RedundantNameQualifier
// ReSharper enable ConvertNullableToShortForm

