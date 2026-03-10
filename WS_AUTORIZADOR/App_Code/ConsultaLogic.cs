using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Globalization;

namespace WCF_Autorizador
{
    public class ConsultaLogic
    {
        public RespuestaTransaccion ProcesarConsulta(string tarjeta, string cvv, string vencimiento, int idCajero)
        {
            RespuestaTransaccion respuesta = new RespuestaTransaccion();
            try
            {
                // 1. Limpieza de tarjeta
                string tarjetaLimpia = new string(tarjeta.Where(char.IsDigit).ToArray());

                // 2. Trama para Python (Tipo 2 = Consulta)
                string trama = "2" + tarjetaLimpia.PadRight(16) + "00000000" + "1515" + new string(' ', 26);

                using (TcpClient client = new TcpClient("127.0.0.1", 5001))
                {
                    NetworkStream stream = client.GetStream();
                    byte[] data = Encoding.UTF8.GetBytes(trama);
                    stream.Write(data, 0, data.Length);

                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string respuestaSocket = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                    // 3. Procesar el JSON: {"estado": "APROBADO", ..., "saldo": 538524.5}
                    if (respuestaSocket.Contains("APROBADO"))
                    {
                        // Buscamos la posición del saldo en el JSON de forma manual para evitar librerías extra
                        string claveSaldo = "\"saldo\":";
                        int indexSaldo = respuestaSocket.IndexOf(claveSaldo);

                        if (indexSaldo != -1)
                        {
                            int inicioValor = indexSaldo + claveSaldo.Length;
                            // El valor termina en la coma o en la llave de cierre
                            int finValor = respuestaSocket.IndexOfAny(new char[] { ',', '}' }, inicioValor);

                            string valorExtraido = respuestaSocket.Substring(inicioValor, finValor - inicioValor).Trim();

                            double saldoFinal = 0;
                            // Importante: InvariantCulture porque el JSON trae punto decimal (.)
                            if (double.TryParse(valorExtraido, NumberStyles.Any, CultureInfo.InvariantCulture, out saldoFinal))
                            {
                                respuesta.Resultado = true;
                                respuesta.Mensaje = "Transacción exitosa";
                                // Formato CUC: ₡538,524.50
                                respuesta.Saldo = saldoFinal.ToString("N2", CultureInfo.GetCultureInfo("es-CR"));
                                respuesta.CodigoAutorizacion = "CONS-" + DateTime.Now.ToString("mmss");
                            }
                        }
                    }
                    else
                    {
                        respuesta.Resultado = false;
                        respuesta.Mensaje = "No autorizado por el servidor";
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = false;
                respuesta.Mensaje = "Error de conexión: " + ex.Message;
            }
            return respuesta;
        }
    }
}