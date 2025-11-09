'use client';
import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import {
  Field,
  FieldDescription,
  FieldGroup,
  FieldLabel,
} from "@/components/ui/field"
import { Input } from "@/components/ui/input"
import { fetchlogin } from "@/lib/api";
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { LoginResponse } from '@/lib/types';

export function LoginPage() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const router = useRouter();

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();

    const data: LoginResponse = await fetchlogin(username, password);

    if (data && data.token) {
      router.push('/dashboard');
    } else {
      toast.error(data.message || 'Login failed: Invalid username or password!');
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center p-4">
      <ToastContainer />
      <div className="w-full max-w-xl">
        <Card className="overflow-hidden p-0">
          <CardContent className="p-8">
            <form onSubmit={handleSubmit}>
              <FieldGroup>
                <div className="flex flex-col items-center gap-2 text-center">
                  <h1 className="text-2xl font-bold">Welcome back</h1>
                  <p className="text-muted-foreground text-balance">
                    Enter your username below to login to your account
                  </p>
                </div>
                <Field>
                  <FieldLabel htmlFor="username">Username</FieldLabel>
                  <Input
                    id="username"
                    type="text"
                    placeholder="Nhập tên đăng nhập"
                    required
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                  />
                </Field>
                <Field>
                  <div className="flex items-center">
                    <FieldLabel htmlFor="password">Password</FieldLabel>
                  </div>
                  <Input
                    id="password"
                    type="password"
                    placeholder="Nhập mật khẩu"
                    required
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                  />
                </Field>
                <Field>
                  <Button type="submit" className="w-full">Đăng nhập</Button>
                </Field>
                <FieldDescription className="text-center">
                  Để đăng nhập, hãy liên hệ với Vũ Hà Nam.
                </FieldDescription>
              </FieldGroup>
            </form>
          </CardContent>
        </Card>
        <FieldDescription className="text-center mt-4">
          By clicking continue, you agree to our <a href="#">Terms of Service</a>{" "}
          and <a href="#">Privacy Policy</a>.
        </FieldDescription>
      </div>
    </div>
  );
}
export default LoginPage;