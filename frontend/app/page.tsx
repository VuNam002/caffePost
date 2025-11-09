'use client';
import withAuth from '@/components/withAuth';

function Home() {
  return (
    <main>
      <h1>Welcome to CaffePost!</h1>
    </main>
  );
}

export default withAuth(Home);
