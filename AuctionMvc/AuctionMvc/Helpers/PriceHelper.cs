using System;

namespace AuctionMvc.Helpers
{
  public static class PriceHelper
  {
    public static decimal IntToDecimal(int price)
    {
      return (decimal)price / 100;
    }

    public static int DecimalToInt(decimal price)
    {
      return Convert.ToInt32(price * 100);
    }
  }
}
