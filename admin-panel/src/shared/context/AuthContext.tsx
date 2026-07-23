import { createContext, useState } from "react";
import type { AuthContextType, User } from "../types";
import { loginService } from "../services/auth.service";

// Context lives here alongside the provider to avoid
// a two-file cycle on case-insensitive filesystems.
// eslint-disable-next-line react-refresh/only-export-components
export const AuthContext = createContext<AuthContextType | null>(null);

function parseStoredUser(): User | null {
  try {
    const storedUser = localStorage.getItem("artemis-user");
    const token = localStorage.getItem("artemis-token");

    if (!storedUser || !token) {
      return null;
    }

    const parsed = JSON.parse(storedUser) as User;

    if (!parsed?.id || !parsed?.email) {
      localStorage.removeItem("artemis-user");
      localStorage.removeItem("artemis-token");
      return null;
    }

    return parsed;
  } catch {
    localStorage.removeItem("artemis-user");
    localStorage.removeItem("artemis-token");
    return null;
  }
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(() => parseStoredUser());
  const [loading, setLoading] = useState(false);

  const login = async (email: string, password: string) => {
    setLoading(true);

    try {
      const loginResponse = await loginService.login({
        email,
        password,
      });

      const userInfo: User = {
        id: loginResponse.user.id,
        email: loginResponse.user.email,
        fullName: loginResponse.user.fullName,
        isAdmin: loginResponse.user.isAdmin,
      };

      localStorage.setItem("artemis-token", loginResponse.token);
      localStorage.setItem("artemis-user", JSON.stringify(userInfo));
      setUser(userInfo);
    } finally {
      setLoading(false);
    }
  };

  const logout = () => {
    localStorage.removeItem("artemis-token");
    localStorage.removeItem("artemis-user");
    setUser(null);
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        loading,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}
