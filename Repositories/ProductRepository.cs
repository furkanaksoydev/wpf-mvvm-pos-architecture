using Lavira.AkyaPOS.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Lavira.AkyaPOS.Repositories
{
    public class ProductRepository
    {
        public List<Product> GetAll()
        {
            var products = new List<Product>();

            try
            {
                using var conn = new SQLiteConnection(Core.Database.DatabaseInitializer.ConnectionString);
                conn.Open();

                using var cmd = new SQLiteCommand(
                    "SELECT id, name, price, category_id, company_id FROM akya_products",
                    conn);

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Price = (decimal)reader.GetDouble(2),
                        CategoryId = reader.GetInt32(3),
                        CompanyId = reader.GetInt32(4)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ProductRepository.GetAll Hata: " + ex.Message);
            }

            return products;
        }

        // ✅ EKLENEN METOT
        public List<Product> GetByCompany(int companyId)
        {
            var products = new List<Product>();

            try
            {
                using var conn = new SQLiteConnection(Core.Database.DatabaseInitializer.ConnectionString);
                conn.Open();

                using var cmd = new SQLiteCommand(
                    @"SELECT id, name, price, category_id, company_id 
                      FROM akya_products 
                      WHERE company_id = @companyId", conn);

                cmd.Parameters.AddWithValue("@companyId", companyId);

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Price = (decimal)reader.GetDouble(2),
                        CategoryId = reader.GetInt32(3),
                        CompanyId = reader.GetInt32(4)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ProductRepository.GetByCompany Hata: " + ex.Message);
            }

            return products;
        }
    }
}
