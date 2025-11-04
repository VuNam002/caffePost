export async function fetchlogin(username: string, password: string): Promise<boolean> {
  const response = await fetch('/api/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ username, password }),
  });

  if (response.ok) {
    // const data = await response.json(); // Uncomment if your API returns data on success
    // You might store a token or user info here
    return true;
  } else {
    // Handle API error (e.g., log the error, check status codes for specific errors)
    console.error('Login API error:', response.status, response.statusText);
    return false;
  }
}
