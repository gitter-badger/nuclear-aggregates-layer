﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.Platform.API.ServiceBusBroker {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="BrokerApiError", Namespace="http://schemas.datacontract.org/2004/07/DoubleGis.Integration.Esb")]
    [System.SerializableAttribute()]
    public partial class BrokerApiError : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string[] Errorsk__BackingFieldField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="<Errors>k__BackingField", IsRequired=true)]
        public string[] Errorsk__BackingField {
            get {
                return this.Errorsk__BackingFieldField;
            }
            set {
                if ((object.ReferenceEquals(this.Errorsk__BackingFieldField, value) != true)) {
                    this.Errorsk__BackingFieldField = value;
                    this.RaisePropertyChanged("Errorsk__BackingField");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://2gis.ru/services/brokerapiservice", ConfigurationName="DoubleGis.Erm.Platform.API.ServiceBusBroker.IBrokerApiSender", SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface IBrokerApiSender {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://2gis.ru/services/brokerapiservice/IBrokerApiSender/BeginSending", ReplyAction="http://2gis.ru/services/brokerapiservice/IBrokerApiSender/BeginSendingResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(DoubleGis.Erm.Platform.API.ServiceBusBroker.BrokerApiError), Action="http://2gis.ru/services/brokerapiservice/IBrokerApiSender/BeginSendingBrokerApiEr" +
            "rorFault", Name="BrokerApiError", Namespace="http://schemas.datacontract.org/2004/07/DoubleGis.Integration.Esb")]
        void BeginSending(string appCode, string messageType);
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://2gis.ru/services/brokerapiservice/IBrokerApiSender/SendDataObject", ReplyAction="http://2gis.ru/services/brokerapiservice/IBrokerApiSender/SendDataObjectResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(DoubleGis.Erm.Platform.API.ServiceBusBroker.BrokerApiError), Action="http://2gis.ru/services/brokerapiservice/IBrokerApiSender/SendDataObjectBrokerApi" +
            "ErrorFault", Name="BrokerApiError", Namespace="http://schemas.datacontract.org/2004/07/DoubleGis.Integration.Esb")]
        void SendDataObject(string dataObject);
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://2gis.ru/services/brokerapiservice/IBrokerApiSender/Commit", ReplyAction="http://2gis.ru/services/brokerapiservice/IBrokerApiSender/CommitResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(DoubleGis.Erm.Platform.API.ServiceBusBroker.BrokerApiError), Action="http://2gis.ru/services/brokerapiservice/IBrokerApiSender/CommitBrokerApiErrorFau" +
            "lt", Name="BrokerApiError", Namespace="http://schemas.datacontract.org/2004/07/DoubleGis.Integration.Esb")]
        void Commit();
        
        [System.ServiceModel.OperationContractAttribute(IsTerminating=true, IsInitiating=false, Action="http://2gis.ru/services/brokerapiservice/IBrokerApiSender/EndSending", ReplyAction="http://2gis.ru/services/brokerapiservice/IBrokerApiSender/EndSendingResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(DoubleGis.Erm.Platform.API.ServiceBusBroker.BrokerApiError), Action="http://2gis.ru/services/brokerapiservice/IBrokerApiSender/EndSendingBrokerApiErro" +
            "rFault", Name="BrokerApiError", Namespace="http://schemas.datacontract.org/2004/07/DoubleGis.Integration.Esb")]
        void EndSending();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IBrokerApiSenderChannel : DoubleGis.Erm.Platform.API.ServiceBusBroker.IBrokerApiSender, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BrokerApiSenderClient : System.ServiceModel.ClientBase<DoubleGis.Erm.Platform.API.ServiceBusBroker.IBrokerApiSender>, DoubleGis.Erm.Platform.API.ServiceBusBroker.IBrokerApiSender {
        
        public BrokerApiSenderClient() {
        }
        
        public BrokerApiSenderClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public BrokerApiSenderClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BrokerApiSenderClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BrokerApiSenderClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void BeginSending(string appCode, string messageType) {
            base.Channel.BeginSending(appCode, messageType);
        }
        
        public void SendDataObject(string dataObject) {
            base.Channel.SendDataObject(dataObject);
        }
        
        public void Commit() {
            base.Channel.Commit();
        }
        
        public void EndSending() {
            base.Channel.EndSending();
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://2gis.ru/services/brokerapiservice", ConfigurationName="DoubleGis.Erm.Platform.API.ServiceBusBroker.IBrokerApiReceiver", SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface IBrokerApiReceiver {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://2gis.ru/services/brokerapiservice/IBrokerApiReceiver/BeginReceiving", ReplyAction="http://2gis.ru/services/brokerapiservice/IBrokerApiReceiver/BeginReceivingRespons" +
            "e")]
        [System.ServiceModel.FaultContractAttribute(typeof(DoubleGis.Erm.Platform.API.ServiceBusBroker.BrokerApiError), Action="http://2gis.ru/services/brokerapiservice/IBrokerApiReceiver/BeginReceivingBrokerA" +
            "piErrorFault", Name="BrokerApiError", Namespace="http://schemas.datacontract.org/2004/07/DoubleGis.Integration.Esb")]
        void BeginReceiving(string appCode, string messageType);
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://2gis.ru/services/brokerapiservice/IBrokerApiReceiver/ReceivePackage", ReplyAction="http://2gis.ru/services/brokerapiservice/IBrokerApiReceiver/ReceivePackageRespons" +
            "e")]
        [System.ServiceModel.FaultContractAttribute(typeof(DoubleGis.Erm.Platform.API.ServiceBusBroker.BrokerApiError), Action="http://2gis.ru/services/brokerapiservice/IBrokerApiReceiver/ReceivePackageBrokerA" +
            "piErrorFault", Name="BrokerApiError", Namespace="http://schemas.datacontract.org/2004/07/DoubleGis.Integration.Esb")]
        string[] ReceivePackage();
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://2gis.ru/services/brokerapiservice/IBrokerApiReceiver/Acknowledge", ReplyAction="http://2gis.ru/services/brokerapiservice/IBrokerApiReceiver/AcknowledgeResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(DoubleGis.Erm.Platform.API.ServiceBusBroker.BrokerApiError), Action="http://2gis.ru/services/brokerapiservice/IBrokerApiReceiver/AcknowledgeBrokerApiE" +
            "rrorFault", Name="BrokerApiError", Namespace="http://schemas.datacontract.org/2004/07/DoubleGis.Integration.Esb")]
        void Acknowledge();
        
        [System.ServiceModel.OperationContractAttribute(IsTerminating=true, IsInitiating=false, Action="http://2gis.ru/services/brokerapiservice/IBrokerApiReceiver/EndReceiving", ReplyAction="http://2gis.ru/services/brokerapiservice/IBrokerApiReceiver/EndReceivingResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(DoubleGis.Erm.Platform.API.ServiceBusBroker.BrokerApiError), Action="http://2gis.ru/services/brokerapiservice/IBrokerApiReceiver/EndReceivingBrokerApi" +
            "ErrorFault", Name="BrokerApiError", Namespace="http://schemas.datacontract.org/2004/07/DoubleGis.Integration.Esb")]
        void EndReceiving();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IBrokerApiReceiverChannel : DoubleGis.Erm.Platform.API.ServiceBusBroker.IBrokerApiReceiver, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BrokerApiReceiverClient : System.ServiceModel.ClientBase<DoubleGis.Erm.Platform.API.ServiceBusBroker.IBrokerApiReceiver>, DoubleGis.Erm.Platform.API.ServiceBusBroker.IBrokerApiReceiver {
        
        public BrokerApiReceiverClient() {
        }
        
        public BrokerApiReceiverClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public BrokerApiReceiverClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BrokerApiReceiverClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BrokerApiReceiverClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void BeginReceiving(string appCode, string messageType) {
            base.Channel.BeginReceiving(appCode, messageType);
        }
        
        public string[] ReceivePackage() {
            return base.Channel.ReceivePackage();
        }
        
        public void Acknowledge() {
            base.Channel.Acknowledge();
        }
        
        public void EndReceiving() {
            base.Channel.EndReceiving();
        }
    }
}
