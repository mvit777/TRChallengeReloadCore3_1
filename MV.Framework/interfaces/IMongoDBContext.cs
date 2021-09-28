﻿using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MV.Framework.interfaces
{
    public interface IMongoDBContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
