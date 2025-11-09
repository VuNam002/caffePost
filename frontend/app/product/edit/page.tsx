"use client";

import { useState, useEffect, Suspense } from "react";
import { useSearchParams, useRouter } from "next/navigation";
import { fetchItemById, fetchItemEdit, fetchCategories } from "@/lib/api";
import { ArrowLeft } from "lucide-react";

interface FormData {
  name: string;
  description: string;
  price: number;
  category_id: number;
  image_url: string;
  is_active: boolean;
}

interface Category {
  category_id: number;
  category_name: string;
}

function ItemEditForm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const itemId = searchParams.get("id");

  const [formData, setFormData] = useState<FormData>({
    name: "",
    description: "",
    price: 0,
    category_id: 0, // Initialize with a non-valid value
    image_url: "",
    is_active: true,
  });
  const [categories, setCategories] = useState<Category[]>([]);
  const [itemName, setItemName] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [message, setMessage] = useState<{
    type: "success" | "error";
    text: string;
  } | null>(null);

  useEffect(() => {
    if (!itemId) {
      setMessage({ type: "error", text: "Không tìm thấy ID sản phẩm." });
      setIsLoading(false);
      return;
    }

    const loadInitialData = async () => {
      setIsLoading(true);
      try {
        const [item, cats] = await Promise.all([
          fetchItemById(Number(itemId)),
          fetchCategories(),
        ]);

        if (cats.length > 0) {
          setCategories(cats);
        } else {
          setMessage({
            type: "error",
            text: "Không thể tải danh sách danh mục.",
          });
        }

        if (item) {
          setFormData({
            name: item.name,
            description: item.description,
            price: item.price,
            category_id: item.category_id,
            image_url: item.image_url,
            is_active: item.is_active,
          });
          setItemName(item.name);
        } else {
          setMessage({
            type: "error",
            text: `Không tìm thấy sản phẩm với ID: ${itemId}`,
          });
        }
      } catch (error) {
        console.error("Error fetching initial data:", error);
        setMessage({ type: "error", text: "Không thể tải dữ liệu cho trang." });
      } finally {
        setIsLoading(false);
      }
    };

    loadInitialData();
  }, [itemId]);

  function handleInputChange(
    e: React.ChangeEvent<
      HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
    >
  ) {
    const { name, value, type } = e.target;

    setFormData((prev) => ({
      ...prev,
      [name]:
        type === "checkbox"
          ? (e.target as HTMLInputElement).checked
          : type === "number" || name === "category_id"
          ? parseInt(value, 10)
          : value,
    }));
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();

    if (!itemId) {
      setMessage({ type: "error", text: "ID sản phẩm không hợp lệ." });
      return;
    }
    if (formData.category_id === 0) {
      setMessage({ type: "error", text: "Vui lòng chọn một danh mục." });
      return;
    }

    setIsSaving(true);
    setMessage(null);

    try {
      await fetchItemEdit(Number(itemId), formData);
      setMessage({ type: "success", text: "Cập nhật sản phẩm thành công!" });

      // Redirect back to product list after a short delay
      setTimeout(() => {
        router.push("/product");
      }, 1500);
    } catch (error) {
      console.error("Error updating item:", error);
      setMessage({
        type: "error",
        text: "Có lỗi xảy ra khi cập nhật sản phẩm.",
      });
    } finally {
      setIsSaving(false);
    }
  }

  if (isLoading) {
    <>
      return <div className="text-center p-10">Đang tải dữ liệu...</div>;
    </>;
  }

  return (
    <div className="container mx-auto p-2 max-w-full">
      <button
        onClick={() => router.push("/product")}
        className="flex items-center gap-2 text-gray-600 hover:text-gray-900 mb-4"
      >
        <ArrowLeft size={18} />
        Quay lại danh sách
      </button>
      <h1 className="text-3xl font-bold mb-6">Chỉnh Sửa Sản Phẩm</h1>

      <div className="bg-white rounded-lg shadow-md p-6">
        <h2 className="text-xl font-semibold mb-4">
          {itemName ? `Chỉnh sửa: ${itemName}` : "Không tìm thấy sản phẩm"}
        </h2>

        {message && (
          <div
            className={`mb-4 p-4 rounded-lg ${
              message.type === "success"
                ? "bg-green-100 text-green-800"
                : "bg-red-100 text-red-800"
            }`}
          >
            {message.text}
          </div>
        )}

        {itemId && (
          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label className="block text-sm font-medium mb-1">
                Tên sản phẩm
              </label>
              <input
                type="text"
                name="name"
                value={formData.name}
                onChange={handleInputChange}
                required
                className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-600"
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Mô tả</label>
              <textarea
                name="description"
                value={formData.description}
                onChange={handleInputChange}
                required
                rows={3}
                className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-600"
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">
                Giá (VNĐ)
              </label>
              <input
                type="number"
                name="price"
                value={formData.price}
                onChange={handleInputChange}
                required
                min="0"
                step="1000"
                className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-600"
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Danh mục</label>
              <select
                name="category_id"
                value={formData.category_id}
                onChange={handleInputChange}
                required
                className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-600"
              >
                <option value={0} disabled>
                  -- Chọn danh mục --
                </option>
                {categories.map((cat) => (
                  <option key={cat.category_id} value={cat.category_id}>
                    {cat.category_name}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">
                URL hình ảnh
              </label>
              <input
                type="url"
                name="image_url"
                value={formData.image_url}
                onChange={handleInputChange}
                required
                className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-600"
              />
            </div>

            <div className="flex items-center">
              <input
                type="checkbox"
                name="is_active"
                checked={formData.is_active}
                onChange={handleInputChange}
                className="w-4 h-4 text-black-600 rounded focus:ring-2 focus:ring-gray-600"
              />
              <label className="ml-2 text-sm font-medium">
                Sản phẩm hoạt động
              </label>
            </div>

            <div className="flex gap-3 pt-4">
              <button
                type="submit"
                disabled={isSaving}
                className="flex-1 bg-black text-white py-2 px-4 hover:bg-gray-600 disabled:bg-gray-400 transition-colors"
              >
                {isSaving ? "Đang lưu..." : "Lưu thay đổi"}
              </button>
              <button
                type="button"
                onClick={() => router.back()}
                className="flex-1 bg-gray-200 text-gray-800 py-2 px-4 hover:bg-gray-300 transition-colors"
              >
                Hủy
              </button>
            </div>
          </form>
        )}
      </div>
    </div>
  );
}

export default function ItemEditPage() {
  return (
    <Suspense fallback={<div>Đang tải...</div>}>
      <ItemEditForm />
    </Suspense>
  );
}
