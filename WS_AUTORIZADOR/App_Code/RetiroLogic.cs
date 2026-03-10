using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using WCF_Autorizador;

public class RetiroLogic
{
    public RespuestaTransaccion ProcesarRetiro(string tarjeta, string cvv, string pin, string vencimiento, string cliente, int idCajero, double monto)
    {
        RespuestaTransaccion respuesta = new RespuestaTransaccion();
        try
        {
            // 1. Limpieza de datos
            string tarjetaLimpia = new string(tarjeta.Where(char.IsDigit).ToArray()); // 16 dígitos
            string pinLimpio = pin.Trim(); // 4 dígitos

            // 2. EL CAMBIO CLAVE: Enviamos el monto TAL CUAL (60000)
            // No lo multipliques por 100. Solo rellena con ceros a la izquierda hasta 8 espacios.
            string montoStr = ((long)monto).ToString().PadLeft(8, '0');

            // 3. TRAMA DE 55 CARACTERES
            string tipo = "1";
            string trama = tipo +
                           tarjetaLimpia.PadRight(16) +
                           montoStr + // Aquí irán "00060000"
                           pinLimpio.PadRight(4) +
                           new string(' ', 26);

            // LOG PARA VERIFICAR: En la ventana de Salida de VS deberías ver: 14111111111111111000600001515...
            System.Diagnostics.Debug.WriteLine("TRAMA ENVIADA: [" + trama + "]");

            using (TcpClient client = new TcpClient("127.0.0.1", 5001))
            {
                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(trama);
                stream.Write(data, 0, data.Length);

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string respuestaSocket = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                // Tu test espera "APROBADO"
                if (respuestaSocket.Contains("APROBADO"))
                {
                    respuesta.Resultado = true;
                    respuesta.Mensaje = "Transacción exitosa";
                    respuesta.CodigoAutorizacion = "AUT" + DateTime.Now.Ticks.ToString().Substring(10);
                }
                else
                {
                    respuesta.Resultado = false;
                    respuesta.Mensaje = "No autorizado: " + respuestaSocket;
                }
            }
        }
        catch (Exception ex)
        {
            respuesta.Resultado = false;
            respuesta.Mensaje = "Error: " + ex.Message;
        }
        return respuesta;
    }
}