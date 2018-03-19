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
            BenchmarkRunner.Run<BenchmarkFastQuery>();
        }
    }

    [MemoryDiagnoser]
    public class BenchmarkFastQuery : IDisposable
    {
        private readonly FortuneContext _context = new FortuneContext();

        [Benchmark]
        public void Load()
        {
            _context.Fortunes.Load();
        }

        public void Dispose()
        {
            _context?.Dispose();
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
}
