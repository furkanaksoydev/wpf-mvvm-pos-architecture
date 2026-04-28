using System.Collections.ObjectModel;
using System.Data.SQLite;
using Lavira.AkyaPOS.Core.Database;
using Lavira.AkyaPOS.Core.Models;

namespace Lavira.AkyaPOS.Repositories
{
    public class OrderRepository
    {
        public ObservableCollection<TableOrderItem> GetOpenOrderItems(int tableId)
        {
            var items = new ObservableCollection<TableOrderItem>();

            using var conn = DatabasePathHelper.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT
                    oi.product_id,
                    p.name AS product_name,
                    IFNULL(c.name, 'Ürün') AS category_name,
                    oi.quantity,
                    oi.unit_price
                FROM akya_orders o
                INNER JOIN akya_order_items oi ON oi.order_id = o.id
                INNER JOIN akya_products p ON p.id = oi.product_id
                LEFT JOIN akya_categories c ON c.id = p.category_id
                WHERE o.table_id = @tableId
                  AND o.is_closed = 0
            ";

            cmd.Parameters.AddWithValue("@tableId", tableId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new TableOrderItem
                {
                    ProductId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    CategoryName = reader.GetString(2),
                    Quantity = reader.GetInt32(3),
                    UnitPrice = reader.GetDecimal(4),
                    UnitQuantity = reader.GetInt32(3)
                });
            }

            return items;
        }

        public void UpdateOrderItems(int tableId, ObservableCollection<TableOrderItem> items)
        {
            using var conn = DatabasePathHelper.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            using var getOrderCmd = conn.CreateCommand();
            getOrderCmd.CommandText = @"
                SELECT id FROM akya_orders
                WHERE table_id = @tableId AND is_closed = 0
            ";
            getOrderCmd.Parameters.AddWithValue("@tableId", tableId);

            var orderId = (long?)getOrderCmd.ExecuteScalar();
            if (orderId == null)
                throw new Exception("Açık sipariş bulunamadı.");

            using var deleteCmd = conn.CreateCommand();
            deleteCmd.CommandText =
                "DELETE FROM akya_order_items WHERE order_id = @orderId";
            deleteCmd.Parameters.AddWithValue("@orderId", orderId);
            deleteCmd.ExecuteNonQuery();

            foreach (var item in items)
            {
                using var insertCmd = conn.CreateCommand();
                insertCmd.CommandText = @"
                    INSERT INTO akya_order_items
                    (order_id, product_id, quantity, unit_price)
                    VALUES (@orderId, @productId, @quantity, @unitPrice)
                ";

                insertCmd.Parameters.AddWithValue("@orderId", orderId);
                insertCmd.Parameters.AddWithValue("@productId", item.ProductId);
                insertCmd.Parameters.AddWithValue("@quantity", item.Quantity);
                insertCmd.Parameters.AddWithValue("@unitPrice", item.UnitPrice);

                insertCmd.ExecuteNonQuery();
            }

            tran.Commit();
        }

        public void MoveOrderToTable(int sourceTableId, int targetTableId)
        {
            using var conn = new SQLiteConnection(DatabaseInitializer.ConnectionString);
            conn.Open();

            using var cmd = new SQLiteCommand(@"
        UPDATE akya_orders 
        SET table_id = @targetId 
        WHERE table_id = @sourceId AND is_closed = 0", conn);

            cmd.Parameters.AddWithValue("@targetId", targetTableId);
            cmd.Parameters.AddWithValue("@sourceId", sourceTableId);

            cmd.ExecuteNonQuery();
        }

        public void CloseOrderByManager(int tableId)
        {
            using var conn = new SQLiteConnection(DatabaseInitializer.ConnectionString);
            conn.Open();

            using var cmd = new SQLiteCommand(@"
        UPDATE akya_orders 
        SET is_closed = 2 
        WHERE table_id = @tableId AND is_closed = 0", conn);

            cmd.Parameters.AddWithValue("@tableId", tableId);

            cmd.ExecuteNonQuery();
        }

        public static int? GetActiveOrderId(int tableId)
        {
            using var conn = new SQLiteConnection(DatabaseInitializer.ConnectionString);
            conn.Open();

            using var cmd = new SQLiteCommand(
                @"SELECT id FROM akya_orders 
          WHERE table_id = @tableId AND is_closed = 0 
          ORDER BY id DESC
          LIMIT 1",
                conn);

            cmd.Parameters.AddWithValue("@tableId", tableId);

            var result = cmd.ExecuteScalar();
            if (result == null || result == DBNull.Value)
                return null;

            return Convert.ToInt32(result);
        }
    }
}
