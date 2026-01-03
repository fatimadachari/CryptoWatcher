import { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { getActiveAlerts, createAlert } from '../services/api';
import { useNavigate } from 'react-router-dom';
import LivePrices from '../components/LivePrices';
import { Bell, LogOut, Plus, X, TrendingUp, TrendingDown, CheckCircle2, Loader2, AlertCircle } from 'lucide-react';

export default function Dashboard() {
  const [alerts, setAlerts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showCreateForm, setShowCreateForm] = useState(false);

  const [cryptoSymbol, setCryptoSymbol] = useState('');
  const [targetPrice, setTargetPrice] = useState('');
  const [condition, setCondition] = useState(1);
  const [creating, setCreating] = useState(false);

  const { user, logout } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    loadAlerts();
  }, []);

  const loadAlerts = async () => {
    try {
      setLoading(true);
      const response = await getActiveAlerts();
      setAlerts(response.data);
      setError('');
    } catch (err) {
      setError('Erro ao carregar alertas');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateAlert = async (e) => {
    e.preventDefault();
    setCreating(true);
    setError('');

    try {
      await createAlert(cryptoSymbol.toUpperCase(), parseFloat(targetPrice), parseInt(condition));

      setCryptoSymbol('');
      setTargetPrice('');
      setCondition(1);
      setShowCreateForm(false);

      await loadAlerts();
    } catch (err) {
      setError(err.response?.data?.error || 'Erro ao criar alerta');
    } finally {
      setCreating(false);
    }
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const getConditionIcon = (cond) => {
    return cond === 1 ? <TrendingUp className="w-4 h-4" /> : <TrendingDown className="w-4 h-4" />;
  };

  const getConditionText = (cond) => {
    return cond === 1 ? 'Acima' : 'Abaixo';
  };

  const getStatusBadge = (status) => {
    return status === 1 ? (
      <span className="inline-flex items-center gap-1.5 px-3 py-1 bg-[#00ff88]/10 text-[#00ff88] rounded-full text-xs font-semibold border border-[#00ff88]/30">
        <CheckCircle2 className="w-3 h-3" />
        Ativo
      </span>
    ) : (
      <span className="inline-flex items-center gap-1.5 px-3 py-1 bg-gray-700/50 text-gray-400 rounded-full text-xs font-semibold border border-gray-600">
        <Bell className="w-3 h-3" />
        Disparado
      </span>
    );
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-[#0a0e0d] via-[#0d1410] to-[#0a0e0d] relative">
      {/* Efeito de fundo com linhas neon */}
      <div className="neon-lines"></div>

      {/* Header */}
      <header className="relative z-50 border-b border-[#00ff88]/20 bg-[#0f1914]/60 backdrop-blur-xl sticky top-0 shadow-2xl">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-5">
          <div className="flex justify-between items-center">
            <div className="flex items-center gap-4">
              <div className="p-3 bg-[#00ff88]/10 rounded-xl border border-[#00ff88]/30 animate-glow">
                <TrendingUp className="w-6 h-6 text-[#00ff88]" />
              </div>
              <div>
                <h1 className="text-2xl font-bold text-transparent bg-clip-text bg-gradient-to-r from-white to-[#00ff88]">
                  CryptoWatcher
                </h1>
                <p className="text-sm text-gray-400">
                  Olá, <span className="text-[#00ff88] font-medium">{user?.name}</span>
                </p>
              </div>
            </div>
            <button
              onClick={handleLogout}
              className="flex items-center gap-2 px-5 py-2.5 bg-red-500/10 text-red-400 rounded-xl hover:bg-red-500/20 transition-all border border-red-500/30 font-medium"
            >
              <LogOut className="w-4 h-4" />
              Sair
            </button>
          </div>
        </div>
      </header>

      <main className="relative z-10 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-8">
        <LivePrices />

        <div className="flex justify-between items-center">
          <div className="flex items-center gap-3">
            <div className="p-2.5 bg-[#00ff88]/10 rounded-xl border border-[#00ff88]/30 animate-pulse">
              <Bell className="w-5 h-5 text-[#00ff88]" />
            </div>
            <div>
              <h2 className="text-xl font-bold text-transparent bg-clip-text bg-gradient-to-r from-white to-[#00ff88]">
                Meus Alertas
              </h2>
              <p className="text-sm text-gray-400">Gerencie seus alertas de preço</p>
            </div>
          </div>
          <button
            onClick={() => setShowCreateForm(!showCreateForm)}
            className={`flex items-center gap-2 px-6 py-3 rounded-xl font-semibold transition-all hover:scale-105 ${showCreateForm
                ? 'bg-red-500/10 text-red-400 border border-red-500/30 hover:bg-red-500/20'
                : 'bg-gradient-to-r from-[#00ff88] to-[#00cc6a] text-black shadow-lg shadow-[#00ff88]/30 animate-glow border border-[#00ff88]/40'
              }`}
          >
            {showCreateForm ? (
              <>
                <X className="w-4 h-4" />
                Cancelar
              </>
            ) : (
              <>
                <Plus className="w-4 h-4" />
                Novo Alerta
              </>
            )}
          </button>
        </div>

        {error && (
          <div className="flex items-center gap-3 bg-red-500/10 border border-red-500/30 text-red-400 px-5 py-4 rounded-xl">
            <AlertCircle className="w-5 h-5 flex-shrink-0" />
            <p className="font-medium">{error}</p>
          </div>
        )}

        {showCreateForm && (
          <div className="bg-[#0f1914]/60 backdrop-blur-xl border border-[#00ff88]/20 rounded-xl p-6 shadow-2xl">
            <h3 className="text-lg font-bold text-white mb-6 flex items-center gap-2">
              <Plus className="w-5 h-5 text-[#00ff88]" />
              Criar Novo Alerta
            </h3>
            <form onSubmit={handleCreateAlert} className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div>
                  <label className="block text-gray-300 font-semibold mb-2 text-sm">Criptomoeda</label>
                  <input
                    type="text"
                    value={cryptoSymbol}
                    onChange={(e) => setCryptoSymbol(e.target.value)}
                    className="w-full px-4 py-3 bg-[#0a0e0d] border border-gray-700 rounded-xl focus:ring-2 focus:ring-[#00ff88] focus:border-[#00ff88] text-white placeholder:text-gray-600 transition-all"
                    placeholder="BTC, ETH, SOL..."
                    required
                  />
                </div>

                <div>
                  <label className="block text-gray-300 font-semibold mb-2 text-sm">Preço Alvo</label>
                  <input
                    type="number"
                    step="0.01"
                    value={targetPrice}
                    onChange={(e) => setTargetPrice(e.target.value)}
                    className="w-full px-4 py-3 bg-[#0a0e0d] border border-gray-700 rounded-xl focus:ring-2 focus:ring-[#00ff88] focus:border-[#00ff88] text-white placeholder:text-gray-600 transition-all"
                    placeholder="50000.00"
                    required
                  />
                </div>

                <div>
                  <label className="block text-gray-300 font-semibold mb-2 text-sm">Condição</label>
                  <select
                    value={condition}
                    onChange={(e) => setCondition(parseInt(e.target.value))}
                    className="w-full px-4 py-3 bg-[#0a0e0d] border border-gray-700 rounded-xl focus:ring-2 focus:ring-[#00ff88] focus:border-[#00ff88] text-white transition-all"
                  >
                    <option value={1}>Acima do preço</option>
                    <option value={2}>Abaixo do preço</option>
                  </select>
                </div>
              </div>

              <button
                type="submit"
                disabled={creating}
                className="w-full bg-gradient-to-r from-[#00ff88] to-[#00cc6a] text-black py-4 rounded-xl font-bold hover:from-[#00ff88]/90 hover:to-[#00cc6a]/90 transition-all disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2 shadow-lg shadow-[#00ff88]/30 border border-[#00ff88]/40"
              >
                {creating ? (
                  <>
                    <Loader2 className="w-5 h-5 animate-spin" />
                    Criando...
                  </>
                ) : (
                  <>
                    <CheckCircle2 className="w-5 h-5" />
                    Criar Alerta
                  </>
                )}
              </button>
            </form>
          </div>
        )}

        {loading ? (
          <div className="flex flex-col items-center justify-center py-16">
            <Loader2 className="w-12 h-12 text-[#00ff88] animate-spin mb-4" />
            <p className="text-gray-400 font-medium">Carregando alertas...</p>
          </div>
        ) : alerts.length === 0 ? (
          <div className="bg-[#0f1914]/60 backdrop-blur-xl border border-[#00ff88]/20 rounded-xl p-16 text-center">
            <div className="inline-flex p-5 bg-[#00ff88]/10 rounded-full mb-4 animate-float">
              <Bell className="w-10 h-10 text-[#00ff88]/50" />
            </div>
            <p className="text-white text-lg font-semibold mb-2">Nenhum alerta ativo</p>
            <p className="text-gray-400">Crie seu primeiro alerta para começar!</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {alerts.map((alert) => (
              <div
                key={alert.id}
                className="group bg-[#0f1914]/60 backdrop-blur-xl border border-[#00ff88]/20 rounded-xl p-6 hover:border-[#00ff88]/50 transition-all duration-300 hover:shadow-2xl hover:shadow-[#00ff88]/20 hover:scale-[1.02]"
              >
                <div className="flex justify-between items-start mb-5">
                  <div className="flex items-center gap-3">
                    <div className="p-2.5 bg-[#00ff88]/10 rounded-xl border border-[#00ff88]/30">
                      {getConditionIcon(alert.condition)}
                    </div>
                    <h3 className="text-2xl font-bold text-white uppercase tracking-wide">
                      {alert.cryptoSymbol}
                    </h3>
                  </div>
                  {getStatusBadge(alert.status)}
                </div>

                <div className="space-y-3">
                  <div className="flex justify-between items-center p-4 bg-[#0a0e0d]/50 rounded-xl border border-gray-800">
                    <span className="text-gray-400 text-sm font-medium">Condição:</span>
                    <span className="font-bold text-white flex items-center gap-1.5">
                      {getConditionIcon(alert.condition)}
                      {getConditionText(alert.condition)}
                    </span>
                  </div>

                  <div className="flex justify-between items-center p-4 bg-[#0a0e0d]/50 rounded-xl border border-gray-800">
                    <span className="text-gray-400 text-sm font-medium">Preço Alvo:</span>
                    <span className="font-bold text-[#00ff88] text-lg">${alert.targetPrice.toLocaleString()}</span>
                  </div>

                  <div className="flex justify-between items-center text-sm pt-2 border-t border-gray-800">
                    <span className="text-gray-500">Criado em:</span>
                    <span className="text-gray-400 font-medium">
                      {new Date(alert.createdAt).toLocaleDateString('pt-BR')}
                    </span>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </main>
    </div>
  );
}