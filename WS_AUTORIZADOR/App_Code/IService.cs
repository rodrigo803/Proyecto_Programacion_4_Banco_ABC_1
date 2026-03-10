using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace WCF_Autorizador
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        RespuestaTransaccion WS_AUTORIZADOR1(string tarjeta, string cvv, string pin, string vencimiento, string cliente, int idCajero, double monto);

        [OperationContract]
        RespuestaTransaccion WS_AUTORIZADOR2(string tarjeta, string cvv, string vencimiento, int idCajero);

        [OperationContract]
        RespuestaTransaccion WS_AUTORIZADOR3(string tarjeta, string pinActual, string pinNuevo, string vencimiento, string cvv, int idCajero);
    }

    [DataContract]
    public class RespuestaTransaccion
    {
        [DataMember] public bool Resultado { get; set; }
        [DataMember] public string Mensaje { get; set; }
        [DataMember] public string CodigoAutorizacion { get; set; }
        [DataMember] public string Saldo { get; set; }
    }
}