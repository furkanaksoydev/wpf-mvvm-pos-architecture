using System.Collections.ObjectModel;
using System.Linq;
using Lavira.AkyaPOS.Repositories;
using Lavira.AkyaPOS.Core.Models;
using System.Windows.Input;

namespace Lavira.AkyaPOS.ViewModels
{
    public class TablesViewModel : BaseViewModel
    {
        private readonly TableRepository _tableRepo = new TableRepository();
        private readonly OrderRepository _orderRepo = new OrderRepository();

        public ObservableCollection<TableItem> Tables { get; } = new ObservableCollection<TableItem>();

        private TableItem _selectedTable;
        public TableItem SelectedTable
        {
            get => _selectedTable;
            set
            {
                if (_selectedTable != value)
                {
                    _selectedTable = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isMoveMode;
        public bool IsMoveMode
        {
            get => _isMoveMode;
            set
            {
                _isMoveMode = value;
                OnPropertyChanged();
            }
        }

        private TableItem _sourceTable;
        public TableItem SourceTable
        {
            get => _sourceTable;
            set
            {
                _sourceTable = value;
                OnPropertyChanged();
            }
        }

        public ICommand PayOrderCommand { get; }


        public TablesViewModel()
        {
            LoadTables();

            PayOrderCommand = new RelayCommand(ExecutePayOrder);
        }

        private void ExecutePayOrder()
        {
            if (SelectedTable == null || !SelectedTable.IsOccupied || IsMoveMode) return;

            PaymentRequested?.Invoke(this, SelectedTable);
        }

        public event EventHandler<TableItem> PaymentRequested;

        public void LoadTables()
        {
            int? lastSelectedId = SelectedTable?.Id;
            Tables.Clear();
            int companyId = 1;

            var tables = _tableRepo.GetTablesByCompany(companyId);

            foreach (var table in tables)
            {
                var tableItem = new TableItem
                {
                    Id = table.Id,
                    Name = table.Name,
                    IsOccupied = table.IsActive
                };

                if (table.IsActive)
                {
                    var orderItems = _orderRepo.GetOpenOrderItems(table.Id);
                    foreach (var item in orderItems)
                    {
                        tableItem.Items.Add(item);
                    }

                    tableItem.ActiveOrderId = OrderRepository.GetActiveOrderId(table.Id);
                }

                Tables.Add(tableItem);
            }

            if (lastSelectedId.HasValue)
                SelectedTable = Tables.FirstOrDefault(t => t.Id == lastSelectedId.Value);

            if (SelectedTable == null)
                SelectedTable = Tables.FirstOrDefault();
        }

        public void StartMoveMode()
        {
            if (SelectedTable == null || !SelectedTable.IsOccupied) return;

            SourceTable = SelectedTable;
            IsMoveMode = true;
            SelectedTable = null;
        }

        public void CancelMoveMode()
        {
            IsMoveMode = false;
            SourceTable = null;
            LoadTables();
        }

        public void CompleteMove()
        {
            if (SourceTable == null || SelectedTable == null) return;

            _orderRepo.MoveOrderToTable(SourceTable.Id, SelectedTable.Id);

            _tableRepo.UpdateTableStatus(SourceTable.Id, false);
            _tableRepo.UpdateTableStatus(SelectedTable.Id, true);

            IsMoveMode = false;
            int targetId = SelectedTable.Id;
            SourceTable = null;

            LoadTables();

            SelectedTable = Tables.FirstOrDefault(t => t.Id == targetId);
        }

        public void CloseCurrentTableByManager()
        {
            if (SelectedTable == null || !SelectedTable.IsOccupied) return;

            _orderRepo.CloseOrderByManager(SelectedTable.Id);

            _tableRepo.UpdateTableStatus(SelectedTable.Id, false);

            LoadTables();
        }
        public void RefreshTables() => LoadTables();
    }
}
