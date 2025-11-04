const API_URL = 'http://localhost:5164/api';
export async function fetchlogin(userName: string, password: string): Promise<boolean> {
    const res = await fetch(`${API_URL}/Users/login`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ userName, password }),
    });
    if(!res.ok){
        throw new Error('Failed to fetch login');
    }
    return true;
}