using Ejercicio4Modulo2.Domain.Entities;
using Ejercicio4Modulo2.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ejercicio4Modulo2
{
    internal class Program
    {
        static void Main(string[] args)
        {
           string path = $"{AppDomain.CurrentDomain.BaseDirectory}\\data.txt";

            var optionsBuilder = new DbContextOptionsBuilder<Ejercicio4Modulo2Context>()
                                .UseSqlServer("Data Source=ELIAS;Initial Catalog=Ejercicio4Modulo2;Integrated Security=True;Encrypt=False");

            var context = new Ejercicio4Modulo2Context(optionsBuilder.Options);

            var file = File.ReadAllLines(path);

            var result = context.Parametria.Where(w => w.Id == 1).FirstOrDefault();

            for (int i = 0; i < file.Length; i++)
            {
                string fecha = file[i].Substring(0, 10);
                string codVendedor = file[i].Substring(10,3);
                string valorVenta = file[i].Substring(13,11);
                string empresaGrande = file[i].Substring(24,1);
                string error = "";

                if (!codVendedor.Equals("   "))
                {
                    if (result.Value == fecha)
                    {
                        if (empresaGrande.Equals("S") || empresaGrande.Equals("N"))
                        {
                            VentasMensuales venta = new VentasMensuales();
                            venta.Fecha = DateTime.Parse(fecha);
                            venta.CodVendedor = codVendedor;
                            venta.Venta = Decimal.Parse(valorVenta);

                            if (empresaGrande.Equals("S")) venta.VentaEmpresaGrande = true;
                            else venta.VentaEmpresaGrande = false;

                            context.VentasMensuales.Add(venta);
                        }
                        else
                        {
                            error = "El flag \"Venta a empresa grande\" tiene solo dos valores posibles \"S\" o \"N\" cualquier otro dato es incorrecto";
                            Rechazos rechazo = new Rechazos();
                            rechazo.Error = error;
                            rechazo.RegistroOriginal = file[i];

                            context.Rechazos.Add(rechazo);
                        }
                    }
                    else
                    {
                        error = "La fecha del informe debe ser igual a " + result.Value;
                        Rechazos rechazo = new Rechazos();
                        rechazo.Error = error;
                        rechazo.RegistroOriginal = file[i];

                        context.Rechazos.Add(rechazo);
                    }
                }
                else
                {
                    error = "Siempre debe venir un codigo de vendedor";
                    Rechazos rechazo = new Rechazos();
                    rechazo.Error = error;
                    rechazo.RegistroOriginal = file[i];

                    context.Rechazos.Add(rechazo);
                }
            }

            context.SaveChanges();

            var mayoresCienMil = context.VentasMensuales
                                          .GroupBy(g => g.CodVendedor)
                                          .Select(s => new { codVendedor = s.Key,
                                                             totalVenta = s.Sum(v => v.Venta) 
                                          })
                                          .Where(w => w.totalVenta.CompareTo(decimal.Parse("100.000")) == 1)
                                          .ToList();
            Console.WriteLine("---------------- Ventas mayores a 100.000 ----------------");
            mayoresCienMil.ForEach(m =>
            {
                Console.WriteLine($"El vendedor {m.codVendedor} vendio {m.totalVenta}");
            });

            var menoresCienMil = context.VentasMensuales
                                          .GroupBy(g => g.CodVendedor)
                                          .Select(s => new {
                                              codVendedor = s.Key,
                                              totalVenta = s.Sum(v => v.Venta)
                                          })
                                          .Where(w => w.totalVenta.CompareTo(decimal.Parse("100.000")) == -1)
                                          .ToList();

            Console.WriteLine("---------------- Ventas menores a 100.000 ----------------");
            menoresCienMil.ForEach(m =>
            {
                Console.WriteLine($"El vendedor {m.codVendedor} vendio {m.totalVenta}");
            });


            Console.WriteLine("---------------- Vendieron a empresas grandes ----------------");
            var vendioAEmpresaGrande = context.VentasMensuales
                .Where(w => w.VentaEmpresaGrande.Equals(bool.Parse("true")))
                .GroupBy(g => g.CodVendedor)
                .Select(s => new { codVendedor = s.Key })
                .ToList();

            vendioAEmpresaGrande.ForEach(m =>
            {
                Console.WriteLine($"{m.codVendedor}");
            });

            Console.WriteLine("---------------- Rechazos ----------------");

            var rechazos = context.Rechazos.ToList();
            rechazos.ForEach(rechazos =>
            {
                Console.WriteLine($"{rechazos.Id} | {rechazos.Error} | {rechazos.RegistroOriginal}");
            });
        }
    }
}