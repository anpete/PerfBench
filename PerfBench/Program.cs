// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.EntityFrameworkCore;

namespace PerfBench
{
    public class Program
    {
        public static void Main()
        {
            // using (var context = new NorthwindContext())
            // {
            //     foreach (var orderDetail in context.OrderDetails)
            //     {
            //         Console.WriteLine(orderDetail.OrderID);
            //     }
            // }

            using (var b = new BenchmarkFastQuery())
            {
                for (var i = 0; i < 5000; i++)
                {
                    b.OrderDetails();

                    if (i % 1000 == 0)
                    {
                        Console.WriteLine($"Processed {i} queries.");
                    }
                }
            }

            Console.WriteLine("Take snapshot");
            Console.ReadKey();

            // BenchmarkRunner.Run<BenchmarkFastQuery>();
        }
    }

    [MemoryDiagnoser]
    public class BenchmarkFastQuery : IDisposable
    {
        private readonly FortuneContext _fortuneContext = new FortuneContext();
        private readonly NorthwindContext _northwindContext = new NorthwindContext();

        //        [Benchmark]
        //        public void Fortune()
        //        {
        //            _fortuneContext.Fortunes.Load();
        //        }

        [Benchmark]
        public void OrderDetails()
        {
            _northwindContext.OrderDetails.Load();
        }

        public void Dispose()
        {
            _fortuneContext?.Dispose();
            _northwindContext?.Dispose();
        }
    }

    public class FortuneContext : DbContext
    {
        public DbQuery<Fortune> Fortunes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Data Source=(localdb)\\MSSQLLocalDB;Database=aspnet5-Benchmarks;Integrated Security=True;Connect Timeout=60;ConnectRetryCount=0");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Query<Fortune>().ToView("fortune");
        }
    }

    public class Fortune
    {
        public int Id { get; set; }
        public string Message { get; set; }
    }

    public class NorthwindContext : DbContext
    {
        public DbQuery<OrderDetail> OrderDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Data Source=(localdb)\\MSSQLLocalDB;Database=Northwind;Integrated Security=True;Connect Timeout=60;ConnectRetryCount=0");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Query<OrderDetail>().ToView("Order Details");
        }
    }

    public class OrderDetail
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }
    }
}
