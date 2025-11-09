"use client";

import { useState, useEffect } from "react";
import { fetchItemCreate, fetchCategories } from "@/lib/api";

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

const CreateItem: React.FC = () => {
    const [categories, setCategories] = useState<Category[]>([]);
    const [formData, setFormData] = useState<FormData>({
        name: '',
        description: '',
        price: 0,
        category_id: 0,
        image_url: '',
        is_active: true
    });
    const [isLoading, setIsLoading] = useState(false);
    const [message, setMessage] = useState<{ type: 'success' | 'error', text: string } | null>(null);
    const [imagePreview, setImagePreview] = useState<string>('');

    useEffect(() => {
        loadCategories();
    }, []);

    async function loadCategories() {
        try {
            const data = await fetchCategories();
            setCategories(data);
            if (data.length > 0) {
                setFormData(prev => ({ ...prev, category_id: data[0].category_id }));
            }
        } catch (error) {
            console.error('Error fetching categories:', error);
            setMessage({ type: 'error', text: 'Không thể tải danh mục' });
        }
    }

    function handleInputChange(e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) {
        const { name, value, type } = e.target;
        
        setFormData(prev => ({
            ...prev,
            [name]: type === 'checkbox' 
                ? (e.target as HTMLInputElement).checked 
                : type === 'number' 
                    ? parseFloat(value) || 0 
                    : value
        }));
        if (name === 'image_url' && value) {
            setImagePreview(value);
        }
    }

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        if (!formData.name.trim()) {
            setMessage({ type: 'error', text: 'Vui lòng nhập tên sản phẩm' });
            return;
        }
        if (formData.price <= 0) {
            setMessage({ type: 'error', text: 'Giá sản phẩm phải lớn hơn 0' });
            return;
        }
        if (!formData.category_id) {
            setMessage({ type: 'error', text: 'Vui lòng chọn danh mục' });
            return;
        }

        setIsLoading(true);
        setMessage(null);

        try {
            await fetchItemCreate(formData);
            
            setMessage({ type: 'success', text: 'Tạo sản phẩm mới thành công!' });
            setTimeout(() => {
                handleReset();
            }, 2000);
        } catch (error: any) {
            console.error('Error creating item:', error);
            setMessage({ type: 'error', text: error.message || 'Có lỗi xảy ra khi tạo sản phẩm' });
        } finally {
            setIsLoading(false);
        }
    }

    function handleReset() {
        setFormData({
            name: '',
            description: '',
            price: 0,
            category_id: categories.length > 0 ? categories[0].category_id : 0,
            image_url: '',
            is_active: true
        });
        setImagePreview('');
        setMessage(null);
    }

    return (
        <div className="container mx-full p-6 max-full">
            <div className="bg-white rounded-lg shadow-md p-6">
                <h1 className="text-3xl font-bold mb-6">Tạo Sản Phẩm Mới</h1>

                {message && (
                    <div className={`mb-6 p-4 rounded-lg ${
                        message.type === 'success' 
                            ? 'bg-green-100 text-green-800 border border-green-200' 
                            : 'bg-red-100 text-red-800 border border-red-200'
                    }`}>
                        <div className="flex items-center">
                            <span className="font-semibold mr-2">
                                {message.type === 'success' ? '✓' : '✗'}
                            </span>
                            {message.text}
                        </div>
                    </div>
                )}

                <form onSubmit={handleSubmit} className="space-y-6">
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        <div className="space-y-4">
                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-2">
                                    Tên sản phẩm <span className="text-red-500">*</span>
                                </label>
                                <input
                                    type="text"
                                    name="name"
                                    value={formData.name}
                                    onChange={handleInputChange}
                                    required
                                    placeholder="Nhập tên sản phẩm"
                                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-600 focus:border-transparent"
                                />
                            </div>

                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-2">
                                    Mô tả <span className="text-red-500">*</span>
                                </label>
                                <textarea
                                    name="description"
                                    value={formData.description}
                                    onChange={handleInputChange}
                                    required
                                    rows={4}
                                    placeholder="Nhập mô tả sản phẩm"
                                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-600 focus:border-transparent resize-none"
                                />
                            </div>

                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-2">
                                    Giá (VNĐ) <span className="text-red-500">*</span>
                                </label>
                                <input
                                    type="number"
                                    name="price"
                                    value={formData.price || ''}
                                    onChange={handleInputChange}
                                    required
                                    min="0"
                                    step="1000"
                                    placeholder="0"
                                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-600 focus:border-transparent"
                                />
                                {formData.price > 0 && (
                                    <p className="mt-1 text-sm text-gray-600">
                                        {formData.price.toLocaleString('vi-VN')} đ
                                    </p>
                                )}
                            </div>

                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-2">
                                    Danh mục <span className="text-red-500">*</span>
                                </label>
                                <select
                                    name="category_id"
                                    value={formData.category_id}
                                    onChange={handleInputChange}
                                    required
                                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-600 focus:border-transparent"
                                >
                                    <option value="">-- Chọn danh mục --</option>
                                    {categories.map((category) => (
                                        <option key={category.category_id} value={category.category_id}>
                                            {category.category_name}
                                        </option>
                                    ))}
                                </select>
                            </div>

                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-2">
                                    URL hình ảnh <span className="text-red-500">*</span>
                                </label>
                                <input
                                    type="url"
                                    name="image_url"
                                    value={formData.image_url}
                                    onChange={handleInputChange}
                                    required
                                    placeholder="https://example.com/image.jpg"
                                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-600 focus:border-transparent"
                                />
                            </div>

                            <div className="flex items-center pt-2">
                                <input
                                    type="checkbox"
                                    name="is_active"
                                    checked={formData.is_active}
                                    onChange={handleInputChange}
                                    className="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-2 focus:ring-gray-600"
                                />
                                <label className="ml-2 text-sm font-medium text-gray-700">
                                    Kích hoạt sản phẩm ngay
                                </label>
                            </div>
                        </div>

                        <div className="space-y-4">
                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-2">
                                    Xem trước
                                </label>
                                <div className="border-2 border-dashed border-gray-300 rounded-lg p-4 bg-gray-50">
                                    {imagePreview ? (
                                        <div className="space-y-3">
                                          {/* eslint-disable-next-line @next/next/no-img-element */}
                                            <img
                                                src={imagePreview}
                                                alt="Preview"
                                                className="w-full h-64 object-cover rounded-lg"
                                                onError={(e) => {
                                                    (e.target as HTMLImageElement).src = 'https://via.placeholder.com/400x300?text=Invalid+Image+URL';
                                                }}
                                            />
                                            <div className="bg-white p-4 rounded-lg shadow-sm">
                                                <h3 className="font-semibold text-lg mb-2">
                                                    {formData.name || 'Tên sản phẩm'}
                                                </h3>
                                                <p className="text-sm text-gray-600 mb-3">
                                                    {formData.description || 'Mô tả sản phẩm'}
                                                </p>
                                                <div className="flex items-center justify-between">
                                                    <span className="text-xl font-bold text-blue-600">
                                                        {formData.price > 0 
                                                            ? `${formData.price.toLocaleString('vi-VN')} đ` 
                                                            : '0 đ'
                                                        }
                                                    </span>
                                                    <span className={`px-3 py-1 rounded-full text-xs font-medium ${
                                                        formData.is_active 
                                                            ? 'bg-green-100 text-green-800' 
                                                            : 'bg-gray-100 text-gray-800'
                                                    }`}>
                                                        {formData.is_active ? 'Hoạt động' : 'Ngưng'}
                                                    </span>
                                                </div>
                                            </div>
                                        </div>
                                    ) : (
                                        <div className="flex flex-col items-center justify-center h-64 text-gray-400">
                                            <svg className="w-16 h-16 mb-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                                            </svg>
                                            <p className="text-sm">Nhập URL hình ảnh để xem trước</p>
                                        </div>
                                    )}
                                </div>
                            </div>
                        </div>
                    </div>

                    <div className="flex gap-4 pt-6 border-t">
                        <button
                            type="submit"
                            disabled={isLoading}
                            className="flex-1 bg-black text-white py-3 px-6 font-medium hover:bg-gray-700 disabled:bg-gray-300 disabled:cursor-not-allowed transition-colors"
                        >
                            {isLoading ? (
                                <span className="flex items-center justify-center">
                                    <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                    </svg>
                                    Đang tạo...
                                </span>
                            ) : (
                                'Tạo sản phẩm'
                            )}
                        </button>
                        <button
                            type="button"
                            onClick={handleReset}
                            disabled={isLoading}
                            className="flex-1 bg-gray-200 text-gray-800 py-3 px-6 rounded-lg font-medium hover:bg-gray-300 disabled:bg-gray-100 disabled:cursor-not-allowed transition-colors"
                        >
                            Làm mới
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default CreateItem;