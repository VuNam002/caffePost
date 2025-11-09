'use client';
import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { getToken } from '@/lib/api';

const withAuth = <P extends object>(WrappedComponent: React.ComponentType<P>) => {
  const WithAuthComponent = (props: P) => {
    const router = useRouter();

    useEffect(() => {
      const token = getToken();
      if (!token) {
        router.replace('/login');
      }
    }, [router]);

    return <WrappedComponent {...props} />;
  };

  return WithAuthComponent;
};

export default withAuth;
