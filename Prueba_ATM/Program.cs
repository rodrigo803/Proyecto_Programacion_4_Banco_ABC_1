using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Asegúrate de que el nombre del espacio de nombres coincida con tu proyecto
using Prueba_ATM.ServicioAutorizador;

namespace Prueba_ATM
{
    class Program
    {
        // Instancia estática del cliente WCF
        static ServiceClient cliente = new ServiceClient();

        static void Main(string[] args)
        {
            bool continuar = true;

            while (continuar)
            {
                Console.Clear();
                Console.WriteLine("====================================================");
                Console.WriteLine("       SISTEMA BANCARIO CUC - NESTOR LAPTOP         ");
                Console.WriteLine("====================================================");
                Console.WriteLine("1. Realizar Retiro (Historia 1)");
                Console.WriteLine("2. Consultar Saldo (Historia 2)");
                Console.WriteLine("3. Cambio de PIN (Historia 3)");
                Console.WriteLine("4. Salir");
                Console.WriteLine("====================================================");
                Console.Write("\nSeleccione una opción: ");

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        EjecutarRetiro();
                        break;
                    case "2":
                        EjecutarConsulta();
                        break;
                    case "3":
                        EjecutarCambioPIN();
                        break;
                    case "4":
                        continuar = false;
                        break;
                    default:
                        Console.WriteLine("\nOpción no válida. Presione una tecla...");
                        Console.ReadKey();
                        break;
                }
            }
            cliente.Close();
        }

        static void EjecutarRetiro()
        {
            Console.WriteLine("\n--- DATOS DE RETIRO (HISTORIA 1) ---");
            Console.Write("Tarjeta: "); string t = Console.ReadLine();
            Console.Write("CVV: "); string c = Console.ReadLine();
            Console.Write("PIN: "); string p = Console.ReadLine();
            Console.Write("Vencimiento (MM/YY): "); string v = Console.ReadLine();
            Console.Write("Cliente: "); string cl = Console.ReadLine();
            Console.Write("ID Cajero: "); int id = int.Parse(Console.ReadLine());
            Console.Write("Monto a retirar: "); double m = double.Parse(Console.ReadLine());

            var res = cliente.WS_AUTORIZADOR1(t, c, p, v, cl, id, m);

            MostrarResultado(res);
        }

        static void EjecutarConsulta()
        {
            Console.WriteLine("\n--- DATOS DE CONSULTA (HISTORIA 2) ---");
            Console.Write("Tarjeta: "); string t = Console.ReadLine();
            Console.Write("CVV: "); string c = Console.ReadLine();
            Console.Write("Vencimiento (MM/YY): "); string v = Console.ReadLine();
            Console.Write("ID Cajero: "); int id = int.Parse(Console.ReadLine());

            var res = cliente.WS_AUTORIZADOR2(t, c, v, id);

            if (res.Resultado)
            {
                Console.WriteLine("\n>>> " + res.Mensaje);
                Console.WriteLine(">>> Saldo Actual: ₡" + res.Saldo);
                Console.WriteLine(">>> Cod. Autorizacion: " + res.CodigoAutorizacion);
            }
            else
            {
                Console.WriteLine("\n>>> ERROR: " + res.Mensaje);
            }
            EsperarTecla();
        }

        static void EjecutarCambioPIN()
        {
            Console.WriteLine("\n--- CAMBIO DE PIN (HISTORIA 3) ---");
            Console.Write("Tarjeta: "); string t = Console.ReadLine();
            Console.Write("PIN Actual: "); string pa = Console.ReadLine();
            Console.Write("PIN Nuevo: "); string pn = Console.ReadLine();
            Console.Write("Vencimiento (MM/YY): "); string v = Console.ReadLine();
            Console.Write("CVV: "); string c = Console.ReadLine();
            Console.Write("ID Cajero: "); int id = int.Parse(Console.ReadLine());

            // Llamada al nuevo método del WCF
            var res = cliente.WS_AUTORIZADOR3(t, pa, pn, v, c, id);

            if (res.Resultado)
            {
                Console.WriteLine("\n>>> " + res.Mensaje);
                Console.WriteLine(">>> Comprobante: " + res.CodigoAutorizacion);
            }
            else
            {
                Console.WriteLine("\n>>> ERROR: " + res.Mensaje);
            }
            EsperarTecla();
        }

        static void MostrarResultado(RespuestaTransaccion res)
        {
            if (res.Resultado)
            {
                Console.WriteLine("\n>>> EXITO: " + res.Mensaje);
                Console.WriteLine(">>> Cod. Autorizacion: " + res.CodigoAutorizacion);
            }
            else
            {
                Console.WriteLine("\n>>> ERROR: " + res.Mensaje);
            }
            EsperarTecla();
        }

        static void EsperarTecla()
        {
            Console.WriteLine("\nPresione cualquier tecla para volver al menú...");
            Console.ReadKey();
        }
    }
}