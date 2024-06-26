﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DemoApp;

public class RulesEngineContext : DbContext
{
    public RulesEngineContext()
    {
        var folder = Assembly.GetExecutingAssembly().Location;
        var path = Path.GetDirectoryName(folder);
        DbPath = $"{path}{Path.DirectorySeparatorChar}RulesEngineDemo.db";
    }

    public string DbPath { get; }

    public DbSet<Workflow> Workflows { get; set; }
    public DbSet<Rule> Rules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ScopedParam>()
            .HasKey(k => k.Name);

        modelBuilder.Entity<Workflow>(entity => {
            entity.HasKey(k => k.WorkflowName);
            entity.Ignore(b => b.WorkflowsToInject);
        });

        modelBuilder.Entity<Rule>().HasOne<Rule>().WithMany(r => r.Rules).HasForeignKey("RuleNameFK");

        modelBuilder.Entity<Rule>(entity => {
            entity.HasKey(k => k.RuleName);

            var valueComparer = new ValueComparer<Dictionary<string, object>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c);

                entity.Property(b => b.Properties).HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, JsonSerializerOptions.Default))
                .Metadata
                .SetValueComparer(valueComparer);

                entity.Property(p => p.Actions).HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<RuleActions>(v, JsonSerializerOptions.Default));

            entity.Ignore(b => b.WorkflowsToInject);
        });
    }
}