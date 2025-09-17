using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

class Program
{
    static List<Empleado> empleados = new();

    static void Main()
    {
        bool salir = false;

        while (!salir)
        {
            Console.Clear();
            Console.WriteLine("=== Gestión de Pagos de Empleados ===");
            Console.WriteLine("1. Capturar empleado");
            Console.WriteLine("2. Actualizar empleado");
            Console.WriteLine("3. Calcular pagos");
            Console.WriteLine("4. Generar reporte");
            Console.WriteLine("5. Salir");
            Console.Write("Seleccione una opción: ");
            string opcion = Console.ReadLine()!;

            switch (opcion)
            {
                case "1": CapturarEmpleado(); break;
                case "2": ActualizarEmpleado(); break;
                case "3": CalcularPagos(); break;
                case "4": GenerarReporte(); break;
                case "5": salir = true; break;
                default: Console.WriteLine("Opción inválida."); Console.ReadKey(); break;
            }
        }
    }

    static void CapturarEmpleado()
    {
        Console.Clear();
        Console.WriteLine("Tipo de empleado:");
        Console.WriteLine("1. Asalariado");
        Console.WriteLine("2. Por Horas");
        Console.WriteLine("3. Por Comisión");
        Console.WriteLine("4. Asalariado por Comisión");
        Console.Write("Opción: ");
        string tipo = Console.ReadLine()!;

        Console.Write("Primer Nombre: ");
        string nombre = Console.ReadLine()!;
        Console.Write("Apellido Paterno: ");
        string apellido = Console.ReadLine()!;
        Console.Write("Número de Seguro Social: ");
        string ssn = Console.ReadLine()!; // <-- Agregado

        Empleado? empleado = tipo switch
        {
            "1" => new EmpleadoAsalariado(nombre, apellido, ssn, LeerDecimal("Salario Semanal: ")),
            "2" => new EmpleadoPorHoras(nombre, apellido, ssn, LeerDecimal("Sueldo por Hora: "), LeerDouble("Horas Trabajadas: ")),
            "3" => new EmpleadoPorComision(nombre, apellido, ssn, LeerDecimal("Ventas Brutas: "), LeerDecimal("Tarifa Comisión: ")),
            "4" => new EmpleadoAsalariadoPorComision(nombre, apellido, ssn, LeerDecimal("Ventas Brutas: "), LeerDecimal("Tarifa Comisión: "), LeerDecimal("Salario Base: ")),
            _ => null
        };

        if (empleado != null)
        {
            empleados.Add(empleado);
            Console.WriteLine("Empleado registrado.");
        }
        else Console.WriteLine("Tipo inválido.");

        Console.ReadKey();
    }

    static void ActualizarEmpleado()
    {
        Console.Clear();
        Console.Write("Ingrese el número de seguro social del empleado: ");
        string ssn = Console.ReadLine()!;
        var empleado = empleados.FirstOrDefault(e => e.NumeroSeguroSocial == ssn);

        if (empleado == null)
        {
            Console.WriteLine("Empleado no encontrado.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Actualizando datos...");
        if (empleado is EmpleadoAsalariado ea)
            ea.SalarioSemanal = LeerDecimal("Nuevo salario semanal: ");
        else if (empleado is EmpleadoPorHoras eh)
        {
            eh.SueldoPorHora = LeerDecimal("Nuevo sueldo por hora: ");
            eh.HorasTrabajadas = LeerDouble("Nuevas horas trabajadas: ");
        }
        else if (empleado is EmpleadoPorComision ec)
        {
            ec.VentasBrutas = LeerDecimal("Nuevas ventas brutas: ");
            ec.TarifaComision = LeerDecimal("Nueva tarifa de comisión: ");
        }
        else if (empleado is EmpleadoAsalariadoPorComision eac)
        {
            eac.VentasBrutas = LeerDecimal("Nuevas ventas brutas: ");
            eac.TarifaComision = LeerDecimal("Nueva tarifa de comisión: ");
            eac.SalarioBase = LeerDecimal("Nuevo salario base: ");
        }

        Console.WriteLine("Datos actualizados.");
        Console.ReadKey();
    }

    static void CalcularPagos()
    {
        Console.Clear();
        Console.WriteLine("Pagos calculados automáticamente.");
        Console.ReadKey();
    }

    static void GenerarReporte()
    {
        Console.Clear();
        Console.WriteLine("=== Reporte Semanal ===");
        foreach (var e in empleados)
            Console.WriteLine(e.ObtenerResumenPago());
        Console.ReadKey();
    }

    static decimal LeerDecimal(string mensaje)
    {
        Console.Write(mensaje);
        decimal valor;
        while (!decimal.TryParse(Console.ReadLine(), out valor))
            Console.Write("Entrada inválida. Intente de nuevo: ");
        return valor;
    }

    static double LeerDouble(string mensaje)
    {
        Console.Write(mensaje);
        double valor;
        while (!double.TryParse(Console.ReadLine(), out valor))
            Console.Write("Entrada inválida. Intente de nuevo: ");
        return valor;
    }

    // Clases de empleados
    abstract class Empleado
    {
        public string PrimerNombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string NumeroSeguroSocial { get; set; }

        public Empleado(string nombre, string apellido, string ssn)
        {
            PrimerNombre = nombre;
            ApellidoPaterno = apellido;
            NumeroSeguroSocial = ssn;
        }

        public abstract decimal CalcularPagoSemanal();

        public virtual string ObtenerResumenPago()
        {
            return $"{PrimerNombre} {ApellidoPaterno} ({NumeroSeguroSocial}) - Pago: {CalcularPagoSemanal():C}";
        }
    }

    class EmpleadoAsalariado : Empleado
    {
        public decimal SalarioSemanal { get; set; }

        public EmpleadoAsalariado(string nombre, string apellido, string ssn, decimal salario)
            : base(nombre, apellido, ssn) => SalarioSemanal = salario;

        public override decimal CalcularPagoSemanal() => SalarioSemanal;
    }

    class EmpleadoPorHoras : Empleado
    {
        public decimal SueldoPorHora { get; set; }
        public double HorasTrabajadas { get; set; }

        public EmpleadoPorHoras(string nombre, string apellido, string ssn, decimal sueldo, double horas)
            : base(nombre, apellido, ssn)
        {
            SueldoPorHora = sueldo;
            HorasTrabajadas = horas;
        }

        public override decimal CalcularPagoSemanal()
        {
            if (HorasTrabajadas <= 40)
                return SueldoPorHora * (decimal)HorasTrabajadas;
            else
                return (SueldoPorHora * 40) + (SueldoPorHora * 1.5m * (decimal)(HorasTrabajadas - 40));
        }
    }

    class EmpleadoPorComision : Empleado
    {
        public decimal VentasBrutas { get; set; }
        public decimal TarifaComision { get; set; }

        public EmpleadoPorComision(string nombre, string apellido, string ssn, decimal ventas, decimal tarifa)
            : base(nombre, apellido, ssn)
        {
            VentasBrutas = ventas;
            TarifaComision = tarifa;
        }

        public override decimal CalcularPagoSemanal() => VentasBrutas * TarifaComision;
    }

    class EmpleadoAsalariadoPorComision : EmpleadoPorComision
    {
        public decimal SalarioBase { get; set; }

        public EmpleadoAsalariadoPorComision(string nombre, string apellido, string ssn, decimal ventas, decimal tarifa, decimal salarioBase)
            : base(nombre, apellido, ssn, ventas, tarifa)
        {
            SalarioBase = salarioBase;
        }

        public override decimal CalcularPagoSemanal()
        {
            return base.CalcularPagoSemanal() + SalarioBase + (SalarioBase * 0.10m);
        }

        public override string ObtenerResumenPago()
        {
            return $"{PrimerNombre} {ApellidoPaterno} ({NumeroSeguroSocial}) - Pago: {CalcularPagoSemanal():C} [Asalariado + Comisión]";
        }
    }
}