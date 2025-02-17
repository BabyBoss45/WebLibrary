using System;
using System.Collections.Generic;

public class IsbndbBookModel
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string TitleLong { get; set; }
    public string Isbn { get; set; }
    public string Isbn13 { get; set; }
    public string DeweyDecimal { get; set; }
    public string Binding { get; set; }
    public string Publisher { get; set; }
    public string Language { get; set; }
    public DateTime DatePublished { get; set; } // ISO 8601 date format
    public string Edition { get; set; }
    public int Pages { get; set; }
    public string Dimensions { get; set; }
    public DimensionsStructured DimensionsStructured { get; set; } // Nested object
    public string Overview { get; set; }
    public string Image { get; set; }
    public decimal Msrp { get; set; } // Price as a decimal
    public string Excerpt { get; set; }
    public string Synopsis { get; set; }
    public List<string> Authors { get; set; } // Array of strings
    public List<string> Subjects { get; set; } // Array of strings
    public List<string> Reviews { get; set; } // Array of strings
    public List<Price> Prices { get; set; } // Array of nested objects
    public Related Related { get; set; } // Nested object
    public List<OtherIsbn> OtherIsbns { get; set; } 
}

public class DimensionsStructured
{
    public Measurement Length { get; set; }
    public Measurement Width { get; set; }
    public Measurement Height { get; set; }
    public Measurement Weight { get; set; }
}

public class Measurement
{
    public string Unit { get; set; }
    public double Value { get; set; }
}

public class Price
{
    public string Condition { get; set; }
    public string Merchant { get; set; }
    public string MerchantLogo { get; set; }
    public MerchantLogoOffset MerchantLogoOffset { get; set; }
    public string Shipping { get; set; }
    public string PriceAmount { get; set; } // Keeping it as string due to JSON format
    public string Total { get; set; } // Keeping it as string due to JSON format
    public string Link { get; set; }
}

public class MerchantLogoOffset
{
    public string X { get; set; }
    public string Y { get; set; }
}

public class Related
{
    public string Type { get; set; }
}

public class OtherIsbn
{
    public string Isbn { get; set; }
    public string Binding { get; set; }
}
