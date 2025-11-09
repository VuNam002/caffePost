import { LoginResponse } from "./types";

const API_URL = 'http://localhost:5164/api';

export async function fetchlogin(userName: string, password: string): Promise<LoginResponse> {
    try {
        const res = await fetch(`${API_URL}/Users/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ userName, password }),
        });
        const data = await res.json();
        if (data.token) {
            localStorage.setItem('token', data.token);
        }
        return data;
    } catch (error) {
        console.error('Login API error:', error);
        return { message: 'An unexpected error occurred.' };
    }
}

export async function fetchitems(page: number = 1, pageSize: number = 10, keyword: string = ''): Promise<any>  {
    try {
        const params = new URLSearchParams({
            page: page.toString(),
            pageSize: pageSize.toString(),
        });
        if (keyword) {
            params.append('keyword', keyword);
        }

        const res = await fetch(`${API_URL}/Items/paginated?${params.toString()}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${getToken()}`
            },
        })
        if(!res.ok) {
            // Return a default structure in case of an error to prevent crashes
            return { items: [], totalPages: 0, page: 1 };
        };
        const data = await res.json();
        return data;

    } catch (error) {
        console.error('API error fetching items:', error);
        return { items: [], totalPages: 0, page: 1 };
    }
}

export async function fetchItemById(id: number): Promise<any | null> {
    try {
        const res = await fetch(`${API_URL}/Items/${id}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${getToken()}`
            },
        });
        if (!res.ok) {
            return null;
        }
        const data = await res.json();
        return data;
    } catch (error) {
        console.error(`API error fetching item ${id}:`, error);
        return null;
    }
}

export async function fetchItemDeleted(id: number) {
    try {
        const res = await fetch(`${API_URL}/Items/${id}`, {
            method: 'DELETE',
        })
        if(!res.ok) {
            return;
        };
        const data = await res.json();
        return data;
    } catch (error) {
        console.error('Login API error:', error);
    }
}

export async function fetchItemStatus(id: number, status: boolean) {
  const response = await fetch(`${API_URL}/Items/${id}/status`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(status) 
  });
  
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
  
  return response.json();
}

export async function fetchItemEdit(id: number, updateItem: any) {
    try {
        const response = await fetch(`${API_URL}/Items/${id}`, {
            method: 'PATCH',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(updateItem)
        })
        if(!response.ok) {
            return;
        };
        const data = await response.json();
        return data;
        
    } catch (error) {
        console.error('Login API error:', error);
    }
}

export async function fetchItemCreate(newItem: any) {
    try {
        const res = await fetch(`${API_URL}/Items` , {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${getToken()}`
            },
            body: JSON.stringify(newItem)
        })
        if(!res.ok) {
            const errorData = await res.json().catch(() => ({ message: res.statusText }));
            throw new Error(errorData.message || 'Failed to create item');
        };
        const data = await res.json();
        return data;

    } catch (error) {
        console.error('Creation API error:', error);
        throw error;
    }
}

export async function fetchCategories(): Promise<any[]> {
    try {
        const res = await fetch(`${API_URL}/Category`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${getToken()}`
            },
        });
        if (!res.ok) {
            return [];
        }
        const data = await res.json();
        return data;
    } catch (error) {
        console.error('API error fetching categories:', error);
        return [];
    }
}

export function getToken(): string | null {
    return localStorage.getItem('token');
}

export function logout(): void {
    localStorage.removeItem('token');
}

