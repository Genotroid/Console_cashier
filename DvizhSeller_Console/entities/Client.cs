﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DvizhSeller_Console.entities
{
    class Client
    {
        int id;
        string name;
        string phone;

        public Client(int setId, string setName, string setPhone)
        {
            id = setId;
            name = setName;
            phone = setPhone;
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
            }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        public string GetName()
        {
            return name;
        }

        public string GetPhone()
        {
            return phone;
        }

        public int GetId()
        {
            return id;
        }

        public void SetName(string setName)
        {
            name = setName;
        }

        public void SetPhone(string setPhone)
        {
            phone = setPhone;
        }

        public void SetId(int setId)
        {
            id = setId;
        }

        public string GetNameAndPhone()
        {
            return name + phone;
        }
    }
}
