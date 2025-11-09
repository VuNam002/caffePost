"use client";

import { useState, useEffect } from "react";
import { fetchitems, fetchItemDeleted, fetchItemStatus } from "@/lib/api";
import { useRouter } from "next/navigation";
import { CheckCircle, XCircle, Trash2, NotebookPen } from "lucide-react";

export default function ItemsPage() {
  const [items, setItems] = useState<any[]>([]);
  const [selectedRows, setSelectedRows] = useState<number[]>([]);
  const router = useRouter();

  useEffect(() => {
    async function loadItems() {
      try {
        const data = await fetchitems();
        setItems(data);
      } catch (error) {
        console.error("Error fetching items:", error);
      }
    }
    loadItems();
  }, []);

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
        <div className="mb-4 flex justify-end">
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
                    <span className=" px-3 py-1 rounded-lg text-sm text-gray-700 ">
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
                    <button
                      onClick={() => handleDeleteSingle(row.item_id)}
                      className="text-red-600 hover:text-red-800 hover:bg-red-100 p-1 rounded transition-colors font-medium"
                    >
                      <Trash2 />
                    </button>
                    <button
                      onClick={() =>
                        router.push(`/product/edit?id=${row.item_id}`)
                      }
                      className="text-green-600 hover:text-green-800 hover:bg-green-100 p-1 rounded transition-colors font-medium"
                    >
                      <NotebookPen />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </>
  );
}
