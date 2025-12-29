import { createContext, useState, useContext, useEffect } from 'react';
import { loginApi, registerApi } from '../services/api';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Carregar token do localStorage ao iniciar
    const storedToken = localStorage.getItem('token');
    const storedUser = localStorage.getItem('user');
    
    if (storedToken && storedUser) {
      setToken(storedToken);
      setUser(JSON.parse(storedUser));
    }
    setLoading(false);
  }, []);

  const login = async (email, password) => {
    const response = await loginApi(email, password);
    const { userId, name, token: newToken } = response.data;
    
    setToken(newToken);
    setUser({ userId, email, name });
    
    localStorage.setItem('token', newToken);
    localStorage.setItem('user', JSON.stringify({ userId, email, name }));
  };

  const register = async (email, password, name) => {
    const response = await registerApi(email, password, name);
    const { userId, token: newToken } = response.data;
    
    setToken(newToken);
    setUser({ userId, email, name });
    
    localStorage.setItem('token', newToken);
    localStorage.setItem('user', JSON.stringify({ userId, email, name }));
  };

  const logout = () => {
    setToken(null);
    setUser(null);
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  };

  return (
    <AuthContext.Provider value={{ user, token, loading, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);