using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecondDemo.EntityModels;
using SecondDemo.Models;
using System.Collections;
using Avalonia.Controls;
using SecondDemo;
using System.ComponentModel;

namespace SecondDemo.EntityModels
{
    public class DobavContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Clientservice> ClientServices { get; set; }
    }
}
