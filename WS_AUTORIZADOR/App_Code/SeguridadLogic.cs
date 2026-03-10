using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace WCF_Autorizador
{
    public class SeguridadLogic
    {
        public RespuestaTransaccion ProcesarCambioPIN(string tarjeta, string pinActual, string pinNuevo, string vencimiento, string cvv, int idCajero)
        {
            RespuestaTransaccion respuesta = new RespuestaTransaccion();
            try
            {
                // 1. Limpieza de tarjeta
                string tarjetaLimpia = tarjeta.Replace("-", "").Trim();

                // 2. Construcción del JSON usando string.Format (C# 5 Friendly)
                // Usamos {{ y }} para que el formato ignore las llaves del JSON
                string json = string.Format("{{\"tipo\": \"cambio_pin\", \"numero_tarjeta\": \"{0}\", \"pin_actual\": \"{1}\", \"pin_nuevo\": \"{2}\", \"id_cajero\": {3}}}",
                                            tarjetaLimpia, pinActual, pinNuevo, idCajero);

                using (TcpClient client = new TcpClient("127.0.0.1", 5001))
                {
                    NetworkStream stream = client.GetStream();
                    byte[] data = Encoding.UTF8.GetBytes(json);
                    stream.Write(data, 0, data.Length);

                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string respuestaSocket = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                    // 3. Validación de respuesta
                    if (respuestaSocket.ToUpper().Contains("OK") || respuestaSocket.ToUpper().Contains("APROBADO"))
                    {
                        respuesta.Resultado = true;
                        respuesta.Mensaje = "Transacción exitosa";
                        respuesta.CodigoAutorizacion = "PIN-" + DateTime.Now.ToString("mmss");
                    }
                    else
                    {
                        respuesta.Resultado = false;
                        respuesta.Mensaje = "Rechazado: " + respuestaSocket;
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = false;
                respuesta.Mensaje = "Error de red: " + ex.Message;
            }

            return respuesta;
        }
    }
}