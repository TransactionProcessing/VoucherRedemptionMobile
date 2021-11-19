namespace VoucherRedemptionMobile.IntegrationTests.WithAppium.Common
{
    using System;
    using System.Collections.Generic;
    using Castle.Components.DictionaryAdapter;
    using TestClients.Models;

    public class TestingContext
    {
        public List<Voucher> Vouchers = new List<Voucher>();

        public List<(String userName, String password)> Users = new EditableList<(String userName, String password)>();
    }
}