"use client";

import { useState, useEffect } from "react";
import { fetchitems, fetchItemDeleted, fetchItemStatus } from "@/lib/api";
import { useRouter } from "next/navigation";
import { CheckCircle, XCircle, Trash2, NotebookPen, Search, X } from "lucide-react";

export default function ItemsPage() {
  const [items, setItems] = useState<any[]>([]);
  const [selectedRows, setSelectedRows] = useState<number[]>([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(0);
  const [searchTerm, setSearchTerm] = useState('');
  const [searchInput, setSearchInput] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const router = useRouter();

  useEffect(() => {
    async function loadItems() {
      setIsLoading(true);
      try {
        const data = await fetchitems(currentPage, 10, searchTerm);
        setItems(data.items);
        setTotalPages(data.totalPages);
      } catch (error) {
        console.error("Error fetching items:", error);
        setItems([]);
      } finally {
        setIsLoading(false);
      }
    }
    loadItems();
  }, [currentPage, searchTerm]);

  const handleSearch = () => {
    setSearchTerm(searchInput);
    setCurrentPage(1); // Reset về trang 1 khi tìm kiếm
  };

  const handleClearSearch = () => {
    setSearchInput('');
    setSearchTerm('');
    setCurrentPage(1);
  };

  const handleSearchKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      handleSearch();
    }
  };

  const handleDeleteSingle = async (id: number) => {
    if (!window.confirm("Bạn có chắc muốn xóa mục này?")) {
      return;
    }
    try {
      await fetchItemDeleted(id);
      setItems((currentItems) =>
        currentItems.filter((item) => item.item_id !== id)
      );
      setSelectedRows((prev) => prev.filter((rowId) => rowId !== id));
    } catch (error) {
      console.error(`Error deleting item ${id}:`, error);
      alert("Đã xảy ra lỗi khi xóa mục này.");
    }
  };

  const handleStatus = async (id: number) => {
    try {
      const currentItem = items.find((item) => item.item_id === id);
      if (!currentItem) return;
      const newStatus = !currentItem.is_active;
      await fetchItemStatus(id, newStatus);
      setItems((currentItems) =>
        currentItems.map((item) =>
          item.item_id === id ? { ...item, is_active: newStatus } : item
        )
      );
    } catch (error) {
      console.error("Error updating item status:", error);
      alert("Đã xảy ra lỗi khi cập nhật trạng thái.");
    }
  };

  const handleDeleteSelected = async () => {
    if (selectedRows.length === 0) return;
    if (
      !window.confirm(
        `Bạn có chắc muốn xóa ${selectedRows.length} mục đã chọn?`
      )
    ) {
      return;
    }
    try {
      await Promise.all(selectedRows.map((id) => fetchItemDeleted(id)));
      setItems((currentItems) =>
        currentItems.filter((item) => !selectedRows.includes(item.item_id))
      );
      setSelectedRows([]);
    } catch (error) {
      console.error("Error deleting selected items:", error);
      alert("Đã xảy ra lỗi khi xóa hàng loạt.");
    }
  };

  const toggleRow = (id: number) => {
    setSelectedRows((prev) =>
      prev.includes(id) ? prev.filter((rowId) => rowId !== id) : [...prev, id]
    );
  };

  const toggleAll = () => {
    if (selectedRows.length === items.length) {
      setSelectedRows([]);
    } else {
      setSelectedRows(items.map((item) => item.item_id));
    }
  };

  const StatusDisplay = ({ isActive }: { isActive: boolean }) => {
    return (
      <div className="flex items-center gap-2">
        {isActive ? (
          <CheckCircle size={18} className="text-emerald-600" />
        ) : (
          <XCircle size={18} className="text-rose-600" />
        )}
        <span
          className={isActive ? "text-gray-900 font-medium" : "text-gray-700"}
        >
          {isActive ? "Hoạt động" : "Tạm dừng"}
        </span>
      </div>
    );
  };

  return (
    <>
      <div className="text-2xl font-bold py-2">
        Danh sách sản phẩm
      </div>
      <div className="w-full text-gray-900 p-2 bg-white min-h-screen">
        {/* Search Bar */}
        <div className="mb-4 flex items-center gap-3">
          <div className="flex-1 relative">
            <input
              type="text"
              value={searchInput}
              onChange={(e) => setSearchInput(e.target.value)}
              onKeyPress={handleSearchKeyPress}
              placeholder="Tìm kiếm theo tên sản phẩm..."
              className="w-full pl-10 pr-10 py-2 border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-600 focus:border-transparent"
            />
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" size={20} />
            {searchInput && (
              <button
                onClick={handleClearSearch}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
              >
                <X size={20} />
              </button>
            )}
          </div>
          <button
            onClick={handleSearch}
            className="bg-black hover:bg-gray-600 text-white font-medium py-2 px-6 transition-colors flex items-center gap-2"
          >
            <Search size={18} />
            Tìm kiếm
          </button>
        </div>

        {/* Delete Selected Button */}
        <div className="mb-4 flex justify-between items-center">
          <div className="text-sm text-gray-600">
            {searchTerm && (
              <span>
                Kết quả tìm kiếm cho: <span className="font-semibold">"{searchTerm}"</span>
              </span>
            )}
          </div>
          {selectedRows.length > 0 && (
            <button
              onClick={handleDeleteSelected}
              className="flex items-center gap-2 bg-red-600 hover:bg-red-700 text-white font-medium py-2 px-4 rounded-lg transition-colors"
            >
              <Trash2 size={18} />
              Xóa {selectedRows.length} mục đã chọn
            </button>
          )}
        </div>

        {isLoading ? (
          <div className="flex justify-center items-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
          </div>
        ) : items.length === 0 ? (
          <div className="text-center py-12 text-gray-500">
            <p className="text-lg">Không tìm thấy sản phẩm nào</p>
            {searchTerm && (
              <button
                onClick={handleClearSearch}
                className="mt-4 text-blue-600 hover:text-blue-700 underline"
              >
                Xóa bộ lọc tìm kiếm
              </button>
            )}
          </div>
        ) : (
          <div className="overflow-x-auto rounded-lg border border-gray-200 shadow-sm">
            <table className="w-full border-collapse">
              <thead>
                <tr className="border-b border-gray-200 bg-gray-50">
                  <th className="text-left p-4 font-normal text-gray-600">
                    <input
                      type="checkbox"
                      checked={
                        selectedRows.length === items.length && items.length > 0
                      }
                      onChange={toggleAll}
                      className="w-4 h-4 cursor-pointer accent-blue-600"
                    />
                  </th>
                  <th className="text-left p-4 font-normal text-gray-600">
                    Tên sản phẩm
                  </th>
                  <th className="text-left p-4 font-normal text-gray-600">
                    Mô tả
                  </th>
                  <th className="text-left p-4 font-normal text-gray-600">Giá</th>
                  <th className="text-left p-4 font-normal text-gray-600">
                    Danh mục
                  </th>
                  <th className="text-left p-4 font-normal text-gray-600">
                    Trạng thái
                  </th>
                  <th className="text-left p-4 font-normal text-gray-600">
                    Ngày tạo
                  </th>
                  <th className="text-left p-4 font-normal text-gray-600">
                    Chức năng
                  </th>
                </tr>
              </thead>
              <tbody>
                {items.map((row, index) => (
                  <tr
                    key={row.item_id}
                    className={`border-b border-gray-100 transition-colors hover:bg-gray-50 ${
                      index % 2 === 0 ? "bg-white" : "bg-gray-50/30"
                    }`}
                  >
                    <td className="p-4">
                      <input
                        type="checkbox"
                        checked={selectedRows.includes(row.item_id)}
                        onChange={() => toggleRow(row.item_id)}
                        className="w-4 h-4 cursor-pointer accent-blue-600"
                      />
                    </td>
                    
                    <td className="p-4 font-medium text-gray-900">
                      <div className="flex items-center gap-3">
                        {row.image_url && (
                          <img
                            src={row.image_url}
                            className="w-10 h-10 rounded-lg object-cover"
                          />
                        )}
                        <span>{row.name}</span>
                      </div>
                    </td>
                    <td className="p-4 text-gray-700 max-w-xs truncate">
                      {row.description}
                    </td>
                    <td className="p-4 text-gray-900 font-medium">
                      {new Intl.NumberFormat("vi-VN", {
                        style: "currency",
                        currency: "VND",
                      }).format(row.price)}
                    </td>
                    <td className="p-4">
                      <span className="px-3 py-1 rounded-lg text-sm text-gray-700">
                        {row.category_name}
                      </span>
                    </td>
                    <td className="p-4">
                      <button
                        onClick={() => handleStatus(row.item_id)}
                        className=""
                      >
                        <StatusDisplay isActive={row.is_active} />
                      </button>
                    </td>
                    <td className="p-4 text-gray-700 text-sm">
                      {new Date(row.created_at).toLocaleDateString("vi-VN")}
                    </td>

                    <td className="p-4">
                      <div className="flex gap-2">
                        <button
                          onClick={() => handleDeleteSingle(row.item_id)}
                          className="text-red-600 hover:text-red-800 hover:bg-red-100 p-1 rounded transition-colors font-medium"
                        >
                          <Trash2 size={18} />
                        </button>
                        <button
                          onClick={() =>
                            router.push(`/product/edit?id=${row.item_id}`)
                          }
                          className="text-green-600 hover:text-green-800 hover:bg-green-100 p-1 rounded transition-colors font-medium"
                        >
                          <NotebookPen size={18} />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {totalPages > 1 && (
          <div className="mt-4 flex justify-center gap-2">
            <button
              onClick={() => setCurrentPage(prev => Math.max(1, prev - 1))}
              disabled={currentPage === 1}
              className="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Trước
            </button>
            <span className="px-4 py-2">
              Trang {currentPage} / {totalPages}
            </span>
            <button
              onClick={() => setCurrentPage(prev => Math.min(totalPages, prev + 1))}
              disabled={currentPage === totalPages}
              className="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Sau
            </button>
          </div>
        )}
      </div>
    </>
  );
}