'use client';

import { fetchCategories } from "@/lib/api";
import { useEffect, useState } from "react";

export default function CategoryPage() {
    const [caterory, setCategory] = useState<any[]>([]);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        async function loadCategory() {
            setIsLoading(true);
            try {
                const data = await fetchCategories();
                setCategory(data);
            } catch(error) {
                console.error("Eror fetching category: ", error);
                setCategory([]);
            } finally {
                setIsLoading(false);
            }
        }
    },[]);
    if(isLoading) {
        return(
            <>
                <p>Đang tải danh mục...</p>
            </>
        )
    }

    return (
        <>
            <div className="text-2xl font-bold py-2">
                Danh sách sản phẩm
            </div>
            <div></div>
        </>
    )
}