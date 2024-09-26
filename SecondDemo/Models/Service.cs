using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace SecondDemo.Models;

public partial class Service
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public float Cost { get; set; }

    public int Durationinseconds { get; set; }

    public string? Description { get; set; }

    public double? Discount { get; set; }

    public string? Mainimagepath { get; set; }
    public bool HasDiscount { get => Discount != 0; }
    public float CostWithDiscount
    {
        get => (float)(Cost * (100 - Discount));
    }
    public string BackgroundColor
    {
        get { if (HasDiscount) return "LightGreen"; else return "White";}
    }
    public string StringDiscount
    {
        get { return $"* скидка {Discount}%"; }
    }

    public virtual ICollection<Clientservice> Clientservices { get; set; } = new List<Clientservice>();

    public virtual ICollection<Servicephoto> Servicephotos { get; set; } = new List<Servicephoto>();
}
