﻿namespace Efect.Tests
{
    public class Person   
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int  Age { get; set; }
        public string Email { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }
    }
}