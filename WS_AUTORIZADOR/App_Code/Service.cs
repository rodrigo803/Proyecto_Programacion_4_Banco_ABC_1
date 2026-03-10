using System;
using System.Net.Sockets;
using System.Text;
using WCF_Autorizador;

namespace WCF_Autorizador
{
    public class Service : IService
    {
        // HISTORIA 1
        public RespuestaTransaccion WS_AUTORIZADOR1(string tarjeta, string cvv, string pin, string vencimiento, string cliente, int idCajero, double monto)
        {
            RetiroLogic logica = new RetiroLogic();
            return logica.ProcesarRetiro(tarjeta, cvv, pin, vencimiento, cliente, idCajero, monto);
        }

        // HISTORIA 2: Nombre corregido a WS_AUTORIZADOR2 y 4 parámetros
        public RespuestaTransaccion WS_AUTORIZADOR2(string tarjeta, string cvv, string vencimiento, int idCajero)
        {
            ConsultaLogic logica = new ConsultaLogic();
            // Pasamos los parámetros que pide tu lógica de socket
            return logica.ProcesarConsulta(tarjeta, cvv, vencimiento, idCajero);
        }

        // HISTORIA 3: Nombre corregido a WS_AUTORIZADOR3
        public RespuestaTransaccion WS_AUTORIZADOR3(string tarjeta, string pinActual, string pinNuevo, string vencimiento, string cvv, int idCajero)
        {
            SeguridadLogic logica = new SeguridadLogic();
            // Aquí podrías agregar una capa de desencriptación antes de pasar los datos
            return logica.ProcesarCambioPIN(tarjeta, pinActual, pinNuevo, vencimiento, cvv, idCajero);
        }
    }
}