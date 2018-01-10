﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DvizhSeller_Console.entities.interfaces
{
    public interface  CartElement
    {
        int GetCartCount();

        int GetId();

        void SetCartCount(int count);

        double GetPrice();

        string GetName();

        string GetNameAndSku();

        int GetCategoryId();
    }
}
