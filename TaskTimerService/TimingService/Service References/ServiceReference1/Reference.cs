﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace XLPluginBSSummary.ServiceReference1 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.BuildingSiteSummaryServiceSoap")]
    public interface BuildingSiteSummaryServiceSoap {
        
        // CODEGEN: 命名空间 http://tempuri.org/ 的元素名称 HelloWorldResult 以后生成的消息协定未标记为 nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/HelloWorld", ReplyAction="*")]
        XLPluginBSSummary.ServiceReference1.HelloWorldResponse HelloWorld(XLPluginBSSummary.ServiceReference1.HelloWorldRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/HelloWorld", ReplyAction="*")]
        System.Threading.Tasks.Task<XLPluginBSSummary.ServiceReference1.HelloWorldResponse> HelloWorldAsync(XLPluginBSSummary.ServiceReference1.HelloWorldRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/BSSummarySendInfo", ReplyAction="*")]
        void BSSummarySendInfo();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/BSSummarySendInfo", ReplyAction="*")]
        System.Threading.Tasks.Task BSSummarySendInfoAsync();
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class HelloWorldRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="HelloWorld", Namespace="http://tempuri.org/", Order=0)]
        public XLPluginBSSummary.ServiceReference1.HelloWorldRequestBody Body;
        
        public HelloWorldRequest() {
        }
        
        public HelloWorldRequest(XLPluginBSSummary.ServiceReference1.HelloWorldRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class HelloWorldRequestBody {
        
        public HelloWorldRequestBody() {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class HelloWorldResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="HelloWorldResponse", Namespace="http://tempuri.org/", Order=0)]
        public XLPluginBSSummary.ServiceReference1.HelloWorldResponseBody Body;
        
        public HelloWorldResponse() {
        }
        
        public HelloWorldResponse(XLPluginBSSummary.ServiceReference1.HelloWorldResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class HelloWorldResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string HelloWorldResult;
        
        public HelloWorldResponseBody() {
        }
        
        public HelloWorldResponseBody(string HelloWorldResult) {
            this.HelloWorldResult = HelloWorldResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface BuildingSiteSummaryServiceSoapChannel : XLPluginBSSummary.ServiceReference1.BuildingSiteSummaryServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BuildingSiteSummaryServiceSoapClient : System.ServiceModel.ClientBase<XLPluginBSSummary.ServiceReference1.BuildingSiteSummaryServiceSoap>, XLPluginBSSummary.ServiceReference1.BuildingSiteSummaryServiceSoap {
        
        public BuildingSiteSummaryServiceSoapClient() {
        }
        
        public BuildingSiteSummaryServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public BuildingSiteSummaryServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BuildingSiteSummaryServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BuildingSiteSummaryServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        XLPluginBSSummary.ServiceReference1.HelloWorldResponse XLPluginBSSummary.ServiceReference1.BuildingSiteSummaryServiceSoap.HelloWorld(XLPluginBSSummary.ServiceReference1.HelloWorldRequest request) {
            return base.Channel.HelloWorld(request);
        }
        
        public string HelloWorld() {
            XLPluginBSSummary.ServiceReference1.HelloWorldRequest inValue = new XLPluginBSSummary.ServiceReference1.HelloWorldRequest();
            inValue.Body = new XLPluginBSSummary.ServiceReference1.HelloWorldRequestBody();
            XLPluginBSSummary.ServiceReference1.HelloWorldResponse retVal = ((XLPluginBSSummary.ServiceReference1.BuildingSiteSummaryServiceSoap)(this)).HelloWorld(inValue);
            return retVal.Body.HelloWorldResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<XLPluginBSSummary.ServiceReference1.HelloWorldResponse> XLPluginBSSummary.ServiceReference1.BuildingSiteSummaryServiceSoap.HelloWorldAsync(XLPluginBSSummary.ServiceReference1.HelloWorldRequest request) {
            return base.Channel.HelloWorldAsync(request);
        }
        
        public System.Threading.Tasks.Task<XLPluginBSSummary.ServiceReference1.HelloWorldResponse> HelloWorldAsync() {
            XLPluginBSSummary.ServiceReference1.HelloWorldRequest inValue = new XLPluginBSSummary.ServiceReference1.HelloWorldRequest();
            inValue.Body = new XLPluginBSSummary.ServiceReference1.HelloWorldRequestBody();
            return ((XLPluginBSSummary.ServiceReference1.BuildingSiteSummaryServiceSoap)(this)).HelloWorldAsync(inValue);
        }
        
        public void BSSummarySendInfo() {
            base.Channel.BSSummarySendInfo();
        }
        
        public System.Threading.Tasks.Task BSSummarySendInfoAsync() {
            return base.Channel.BSSummarySendInfoAsync();
        }
    }
}
