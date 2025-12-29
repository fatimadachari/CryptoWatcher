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
      <span className="inline-flex items-center gap-1.5 px-3 py-1 bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-400 rounded-full text-xs font-semibold border border-green-200 dark:border-green-800">
        <CheckCircle2 className="w-3 h-3" />
        Ativo
      </span>
    ) : (
      <span className="inline-flex items-center gap-1.5 px-3 py-1 bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded-full text-xs font-semibold border border-gray-200 dark:border-gray-600">
        <Bell className="w-3 h-3" />
        Disparado
      </span>
    );
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-900 via-gray-800 to-black">
      <header className="border-b border-gray-700 bg-gray-900/80 backdrop-blur-xl sticky top-0 z-50 shadow-2xl">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-5">
          <div className="flex justify-between items-center">
            <div className="flex items-center gap-4">
              <div className="p-3 bg-gradient-to-br from-gray-700 to-gray-800 rounded-xl shadow-xl shadow-gray-900/50 animate-glow">
                <Bell className="w-6 h-6 text-white" />
              </div>
              <div>
                <h1 className="text-2xl font-bold bg-gradient-to-r from-gray-100 to-gray-300 bg-clip-text text-transparent">
                  CryptoWatcher
                </h1>
                <p className="text-sm text-gray-400">
                  Olá, <span className="text-gray-200 font-medium">{user?.name}</span>
                </p>
              </div>
            </div>
            <button
              onClick={handleLogout}
              className="flex items-center gap-2 px-5 py-2.5 bg-red-900/30 text-red-400 rounded-xl hover:bg-red-900/50 transition-all border border-red-800 font-medium shadow-lg"
            >
              <LogOut className="w-4 h-4" />
              Sair
            </button>
          </div>
        </div>
      </header>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-8">
        <LivePrices />

        <div className="flex justify-between items-center">
          <div className="flex items-center gap-3">
            <div className="p-2.5 bg-gradient-to-br from-gray-600 to-gray-700 rounded-xl shadow-lg shadow-gray-900/50 animate-pulse">
              <Bell className="w-5 h-5 text-white" />
            </div>
            <div>
              <h2 className="text-xl font-bold bg-gradient-to-r from-gray-100 to-gray-300 bg-clip-text text-transparent">
                Meus Alertas
              </h2>
              <p className="text-sm text-gray-400">Gerencie seus alertas de preço</p>
            </div>
          </div>
          <button
            onClick={() => setShowCreateForm(!showCreateForm)}
            className={`flex items-center gap-2 px-6 py-3 rounded-xl font-semibold transition-all shadow-xl hover:scale-105 ${
              showCreateForm
                ? 'bg-red-900/30 text-red-400 border border-red-800 hover:bg-red-900/50'
                : 'bg-gradient-to-r from-gray-700 to-gray-800 text-white hover:from-gray-600 hover:to-gray-700 shadow-gray-900/40 animate-glow border border-gray-600'
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
          <div className="flex items-center gap-3 bg-red-900/30 border border-red-800 text-red-400 px-5 py-4 rounded-xl shadow-lg">
            <AlertCircle className="w-5 h-5 flex-shrink-0" />
            <p className="font-medium">{error}</p>
          </div>
        )}

        {showCreateForm && (
          <div className="bg-gray-800 border border-gray-700 rounded-xl p-6 shadow-2xl">
            <h3 className="text-lg font-bold text-white mb-6 flex items-center gap-2">
              <Plus className="w-5 h-5 text-gray-300" />
              Criar Novo Alerta
            </h3>
            <form onSubmit={handleCreateAlert} className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div>
                  <label className="block text-white font-semibold mb-2 text-sm">Criptomoeda</label>
                  <input
                    type="text"
                    value={cryptoSymbol}
                    onChange={(e) => setCryptoSymbol(e.target.value)}
                    className="w-full px-4 py-3 bg-gray-900 border border-gray-700 rounded-xl focus:ring-2 focus:ring-gray-500 focus:border-gray-500 text-white placeholder:text-gray-500 transition-all"
                    placeholder="BTC, ETH, SOL..."
                    required
                  />
                </div>

                <div>
                  <label className="block text-white font-semibold mb-2 text-sm">Preço Alvo</label>
                  <input
                    type="number"
                    step="0.01"
                    value={targetPrice}
                    onChange={(e) => setTargetPrice(e.target.value)}
                    className="w-full px-4 py-3 bg-gray-900 border border-gray-700 rounded-xl focus:ring-2 focus:ring-gray-500 focus:border-gray-500 text-white placeholder:text-gray-500 transition-all"
                    placeholder="50000.00"
                    required
                  />
                </div>

                <div>
                  <label className="block text-white font-semibold mb-2 text-sm">Condição</label>
                  <select
                    value={condition}
                    onChange={(e) => setCondition(parseInt(e.target.value))}
                    className="w-full px-4 py-3 bg-gray-900 border border-gray-700 rounded-xl focus:ring-2 focus:ring-gray-500 focus:border-gray-500 text-white transition-all"
                  >
                    <option value={1}>Acima do preço</option>
                    <option value={2}>Abaixo do preço</option>
                  </select>
                </div>
              </div>

              <button
                type="submit"
                disabled={creating}
                className="w-full bg-gradient-to-r from-gray-700 to-gray-800 text-white py-4 rounded-xl font-bold hover:from-gray-600 hover:to-gray-700 transition-all disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2 shadow-xl shadow-gray-900/30 border border-gray-600"
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
            <Loader2 className="w-12 h-12 text-gray-400 animate-spin mb-4" />
            <p className="text-gray-400 font-medium">Carregando alertas...</p>
          </div>
        ) : alerts.length === 0 ? (
          <div className="bg-gray-800 border border-gray-700 rounded-xl p-16 text-center shadow-xl">
            <div className="inline-flex p-5 bg-gray-700/50 rounded-full mb-4 animate-float">
              <Bell className="w-10 h-10 text-gray-500" />
            </div>
            <p className="text-white text-lg font-semibold mb-2">Nenhum alerta ativo</p>
            <p className="text-gray-400">Crie seu primeiro alerta para começar!</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {alerts.map((alert) => (
              <div
                key={alert.id}
                className="group bg-gray-800 border border-gray-700 rounded-xl p-6 hover:border-gray-500 transition-all duration-300 hover:shadow-2xl hover:shadow-gray-900/50 hover:scale-[1.02]"
              >
                <div className="flex justify-between items-start mb-5">
                  <div className="flex items-center gap-3">
                    <div className="p-2.5 bg-gray-700 rounded-xl">
                      {getConditionIcon(alert.condition)}
                    </div>
                    <h3 className="text-2xl font-bold text-white uppercase tracking-wide">
                      {alert.cryptoSymbol}
                    </h3>
                  </div>
                  {getStatusBadge(alert.status)}
                </div>

                <div className="space-y-3">
                  <div className="flex justify-between items-center p-4 bg-gray-900/50 rounded-xl">
                    <span className="text-gray-400 text-sm font-medium">Condição:</span>
                    <span className="font-bold text-white flex items-center gap-1.5">
                      {getConditionIcon(alert.condition)}
                      {getConditionText(alert.condition)}
                    </span>
                  </div>

                  <div className="flex justify-between items-center p-4 bg-gray-900/50 rounded-xl">
                    <span className="text-gray-400 text-sm font-medium">Preço Alvo:</span>
                    <span className="font-bold text-gray-200 text-lg">${alert.targetPrice.toLocaleString()}</span>
                  </div>

                  <div className="flex justify-between items-center text-sm pt-2 border-t border-gray-700">
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