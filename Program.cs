using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using NfeStatusServico4;
using static NfeStatusServico4.NFeStatusServico4SoapClient;
using System.Xml;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace teste
{
    internal class Program
    {
        static  void Main(string[] args)
        {
            try
            {
                //wshttpbinding mybinding = new wshttpbinding();
                //mybinding.security.transport.clientcredentialtype =
                //    httpclientcredentialtype.certificate;

                //// create the endpoint address. note that the machine name
                //// must match the subject or dns field of the x.509 certificate
                //// used to authenticate the service.
                //endpointaddress ea = new
                //    endpointaddress("http://www.portalfiscal.inf.br/nfe/wsdl/nfestatusservico4");


                var service = new NFeStatusServico4SoapClient(EndpointConfiguration.NFeStatusServico4Soap);

                //var service = new NFeStatusServico4SoapClient(myBinding, ea);
                var store = new X509Store(StoreName.My, StoreLocation.CurrentUser); store.Open(OpenFlags.ReadOnly);
                var cert = store.Certificates.Find(X509FindType.FindBySubjectName, "ZUCCHETTI SOFTWARE E SISTEMAS LTDA:03916076000400", true)[0];

                var add = new EndpointAddress(new Uri("https://www.portalfiscal.inf.br/nfe/wsdl/nfestatusservico4"));
                var enc = new TextMessageEncodingBindingElement(MessageVersion.Default, System.Text.Encoding.UTF8);
                var trans = new HttpsTransportBindingElement() { RequireClientCertificate = true };

                var cred = new ClientCredentials(); cred.ClientCertificate.Certificate = cert;

                var bind = new CustomBinding(enc, trans);

                service.ClientCredentials.ClientCertificate.Certificate = cert;

                var end = new ServiceEndpoint(ContractDescription.GetContract(typeof(NFeStatusServico4SoapClient)),bind,add);
                end.EndpointBehaviors.Add(cred);

                var ch = new ChannelFactory<NFeStatusServico4Soap>(end);
                var nfe = ch.CreateChannel();

                var xmlDocument = new XmlDocument();
                //var xmlNode = xmlDocument.CreateNode("xml", "xml", "http://www.portalfiscal.inf.br/nfe/wsdl/NFeStatusServico4");
                xmlDocument.LoadXml(@"<?xml version='1.0' encoding='UTF - 8'?>
                                            <soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
  <soap:Body>
    <nfeDadosMsg xmlns='http://www.portalfiscal.inf.br/nfe/wsdl/NFeStatusServico4'>xml</nfeDadosMsg>
  </soap:Body>
</soap:Envelope>");

                var ret = service.nfeStatusServicoNFAsync(xmlDocument);



                var status = ret.Result.nfeResultMsg;





            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }


        }
    }
}

