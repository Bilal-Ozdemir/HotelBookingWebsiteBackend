// BookingDto.cs
public class BookingDto
{
    public int    Id            { get; set; }
    public string RoomTypeName  { get; set; }
    public DateTime CheckIn     { get; set; }
    public DateTime CheckOut    { get; set; }
    public decimal Price        { get; set; }
    public bool   IsPaid        { get; set; }
}

// PaymentRequestDto.cs
public class PaymentRequestDto
{
    public int     BookingId { get; set; }
    public decimal Amount    { get; set; }
}

// PaymentDto.cs
public class PaymentDto
{
    public int       Id         { get; set; }
    public int       BookingId  { get; set; }
    public decimal   Amount     { get; set; }
    public DateTime  PaymentDate{ get; set; }
}
