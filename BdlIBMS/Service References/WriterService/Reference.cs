﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace BdlIBMS.WriterService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.bdl.com/", ConfigurationName="WriterService.WriterService")]
    public interface WriterService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.bdl.com/WriterService/OpcWrite", ReplyAction="http://www.bdl.com/WriterService/OpcWriteResponse")]
        bool OpcWrite(string itemID, string value);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.bdl.com/WriterService/OpcWrite", ReplyAction="http://www.bdl.com/WriterService/OpcWriteResponse")]
        System.Threading.Tasks.Task<bool> OpcWriteAsync(string itemID, string value);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.bdl.com/WriterService/ModbusWrite", ReplyAction="http://www.bdl.com/WriterService/ModbusWriteResponse")]
        bool ModbusWrite(ushort adr, ushort value, byte funcode, byte slave);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.bdl.com/WriterService/ModbusWrite", ReplyAction="http://www.bdl.com/WriterService/ModbusWriteResponse")]
        System.Threading.Tasks.Task<bool> ModbusWriteAsync(ushort adr, ushort value, byte funcode, byte slave);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.bdl.com/WriterService/BacnetWrite", ReplyAction="http://www.bdl.com/WriterService/BacnetWriteResponse")]
        bool BacnetWrite(uint itemID, uint value);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.bdl.com/WriterService/BacnetWrite", ReplyAction="http://www.bdl.com/WriterService/BacnetWriteResponse")]
        System.Threading.Tasks.Task<bool> BacnetWriteAsync(uint itemID, uint value);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface WriterServiceChannel : BdlIBMS.WriterService.WriterService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WriterServiceClient : System.ServiceModel.ClientBase<BdlIBMS.WriterService.WriterService>, BdlIBMS.WriterService.WriterService {
        
        public WriterServiceClient() {
        }
        
        public WriterServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public WriterServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WriterServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WriterServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool OpcWrite(string itemID, string value) {
            return base.Channel.OpcWrite(itemID, value);
        }
        
        public System.Threading.Tasks.Task<bool> OpcWriteAsync(string itemID, string value) {
            return base.Channel.OpcWriteAsync(itemID, value);
        }
        
        public bool ModbusWrite(ushort adr, ushort value, byte funcode, byte slave) {
            return base.Channel.ModbusWrite(adr, value, funcode, slave);
        }
        
        public System.Threading.Tasks.Task<bool> ModbusWriteAsync(ushort adr, ushort value, byte funcode, byte slave) {
            return base.Channel.ModbusWriteAsync(adr, value, funcode, slave);
        }
        
        public bool BacnetWrite(uint itemID, uint value) {
            return base.Channel.BacnetWrite(itemID, value);
        }
        
        public System.Threading.Tasks.Task<bool> BacnetWriteAsync(uint itemID, uint value) {
            return base.Channel.BacnetWriteAsync(itemID, value);
        }
    }
}
