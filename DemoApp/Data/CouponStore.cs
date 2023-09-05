using DemoApp.Models;

namespace DemoApp.Data
{
    public static class CouponStore
    {
        public static List<Coupon> CouponList = new List<Coupon>
        {
            new Coupon{ Id=1, Name="c1", IsActive=true},
            new Coupon{ Id=2, Name="c2", IsActive=false}
        };
    }
}
