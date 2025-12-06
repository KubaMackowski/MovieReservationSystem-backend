public class Price
{
    public int Id { get; set; }
    public int Showing_Id { get; set; }
    public decimal PriceValue { get; set; }

    public required Showing Showing { get; set; }
}