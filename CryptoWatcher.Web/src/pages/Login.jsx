import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { TrendingUp, Mail, Lock, AlertCircle } from 'lucide-react';

export default function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await login(email, password);
      navigate('/dashboard');
    } catch (err) {
      setError(err.response?.data?.error || 'Erro ao fazer login');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-[#0a0e0d] via-[#0d1410] to-[#0a0e0d] flex items-center justify-center p-4 relative overflow-hidden">
      {/* Efeito de fundo com linhas neon */}
      <div className="neon-lines"></div>
      
      {/* Gradientes de fundo */}
      <div className="absolute top-0 left-0 w-full h-full overflow-hidden pointer-events-none">
        <div className="absolute top-20 -left-20 w-96 h-96 bg-[#00ff88] opacity-10 blur-[120px] rounded-full"></div>
        <div className="absolute bottom-20 -right-20 w-96 h-96 bg-[#00ff88] opacity-10 blur-[120px] rounded-full"></div>
      </div>

      <div className="relative z-10 w-full max-w-md">
        {/* Logo e Título */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 mb-4 bg-[#00ff88]/10 rounded-2xl border border-[#00ff88]/30 animate-glow">
            <TrendingUp className="w-8 h-8 text-[#00ff88]" />
          </div>
          <h1 className="text-4xl font-bold mb-2">
            <span className="text-transparent bg-clip-text bg-gradient-to-r from-white to-[#00ff88]">
              CryptoWatcher
            </span>
          </h1>
          <p className="text-gray-400">Entre para monitorar seus alertas</p>
        </div>

        {/* Card de Login */}
        <div className="bg-[#0f1914]/60 backdrop-blur-xl rounded-2xl border border-[#00ff88]/20 p-8 shadow-2xl">
          {error && (
            <div className="mb-6 p-4 bg-red-500/10 border border-red-500/30 rounded-xl flex items-center gap-3">
              <AlertCircle className="w-5 h-5 text-red-400 flex-shrink-0" />
              <p className="text-red-400 text-sm">{error}</p>
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-5">
            <div>
              <label className="block text-gray-300 font-medium mb-2 text-sm">Email</label>
              <div className="relative">
                <Mail className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-500" />
                <input
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  className="w-full pl-12 pr-4 py-3 bg-[#0a0e0d] border border-gray-700 rounded-xl focus:ring-2 focus:ring-[#00ff88] focus:border-[#00ff88] text-white placeholder:text-gray-600 transition-all"
                  placeholder="seu@email.com"
                  required
                />
              </div>
            </div>

            <div>
              <label className="block text-gray-300 font-medium mb-2 text-sm">Senha</label>
              <div className="relative">
                <Lock className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-500" />
                <input
                  type="password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  className="w-full pl-12 pr-4 py-3 bg-[#0a0e0d] border border-gray-700 rounded-xl focus:ring-2 focus:ring-[#00ff88] focus:border-[#00ff88] text-white placeholder:text-gray-600 transition-all"
                  placeholder="••••••••"
                  required
                />
              </div>
            </div>

            <button
              type="submit"
              disabled={loading}
              className="w-full bg-gradient-to-r from-[#00ff88] to-[#00cc6a] text-black py-3.5 rounded-xl font-bold hover:from-[#00ff88]/90 hover:to-[#00cc6a]/90 transition-all disabled:opacity-50 disabled:cursor-not-allowed shadow-lg shadow-[#00ff88]/30 animate-glow"
            >
              {loading ? (
                <span className="flex items-center justify-center gap-2">
                  <div className="w-5 h-5 border-2 border-black/20 border-t-black rounded-full animate-spin"></div>
                  Entrando...
                </span>
              ) : (
                'Entrar'
              )}
            </button>
          </form>

          <div className="mt-6 text-center">
            <p className="text-gray-400 text-sm">
              Não tem conta?{' '}
              <Link to="/register" className="text-[#00ff88] font-semibold hover:underline">
                Registre-se
              </Link>
            </p>
          </div>
        </div>

        {/* Footer */}
        <p className="text-center text-gray-600 text-xs mt-6">
          Monitoramento em tempo real de criptomoedas
        </p>
      </div>
    </div>
  );
}