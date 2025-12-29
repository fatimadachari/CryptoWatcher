import { useState, useEffect } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Legend } from 'recharts';
import { TrendingUp, TrendingDown, Activity, Clock } from 'lucide-react';
import { getTopCryptos } from '../services/priceService';

export default function LivePrices() {
  const [cryptos, setCryptos] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selectedCrypto, setSelectedCrypto] = useState(null);
  const [lastUpdate, setLastUpdate] = useState(new Date());

  useEffect(() => {
    loadPrices();
    const interval = setInterval(loadPrices, 30000);
    return () => clearInterval(interval);
  }, []);

  const loadPrices = async () => {
    try {
      const data = await getTopCryptos();
      setCryptos(data);
      if (!selectedCrypto && data.length > 0) {
        setSelectedCrypto(data[0]);
      }
      setLastUpdate(new Date());
    } catch (err) {
      console.error('Erro ao carregar preços:', err);
    } finally {
      setLoading(false);
    }
  };

  const formatPrice = (price) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }).format(price);
  };

  const formatChange = (change) => {
    const isPositive = change >= 0;
    return (
      <span className={`flex items-center gap-1 ${isPositive ? 'text-green-400' : 'text-red-400'}`}>
        {isPositive ? <TrendingUp className="w-3 h-3" /> : <TrendingDown className="w-3 h-3" />}
        {Math.abs(change).toFixed(2)}%
      </span>
    );
  };

  const prepareChartData = () => {
    if (!selectedCrypto || !selectedCrypto.sparkline_in_7d) return [];
    const prices = selectedCrypto.sparkline_in_7d.price;
    const step = Math.floor(prices.length / 24);
    return prices
      .filter((_, index) => index % step === 0)
      .map((price, index) => ({
        time: `${index}h`,
        price: price,
      }));
  };

  if (loading) {
    return (
      <div className="bg-gray-800 border border-gray-700 rounded-xl p-8 backdrop-blur-sm shadow-lg">
        <div className="animate-pulse space-y-6">
          <div className="h-6 bg-gray-700 rounded-lg w-1/4"></div>
          <div className="h-80 bg-gray-700 rounded-xl"></div>
        </div>
      </div>
    );
  }

  return (
    <div className="bg-gray-800 border border-gray-700 rounded-xl p-6 backdrop-blur-sm shadow-xl">
      <div className="flex justify-between items-center mb-8">
        <div className="flex items-center gap-3">
          <div className="p-2 bg-gray-700 rounded-lg animate-glow">
            <Activity className="w-6 h-6 text-gray-300" />
          </div>
          <div>
            <h3 className="text-xl font-bold text-white">Mercado em Tempo Real</h3>
            <p className="text-sm text-gray-400">Acompanhe os preços ao vivo</p>
          </div>
        </div>
        <div className="flex items-center gap-2 text-sm text-gray-400 bg-gray-700/50 px-3 py-1.5 rounded-lg">
          <Clock className="w-4 h-4" />
          {lastUpdate.toLocaleTimeString('pt-BR')}
        </div>
      </div>

      <div className="grid grid-cols-2 md:grid-cols-5 gap-3 mb-8">
        {cryptos.slice(0, 10).map((crypto) => (
          <button
            key={crypto.id}
            onClick={() => setSelectedCrypto(crypto)}
            className={`group relative p-4 rounded-xl border transition-all duration-300 ${
              selectedCrypto?.id === crypto.id
                ? 'border-gray-500 bg-gray-700/50 shadow-lg shadow-gray-900/50'
                : 'border-gray-700 hover:border-gray-600 hover:bg-gray-700/30'
            }`}
          >
            <div className="flex items-center gap-2 mb-2">
              <img
                src={crypto.image}
                alt={crypto.symbol}
                className="w-7 h-7 rounded-full ring-2 ring-gray-700 group-hover:ring-gray-600 transition-all"
              />
              <span className="font-bold text-white uppercase tracking-wide">{crypto.symbol}</span>
            </div>
            <div className="text-base font-semibold text-white mb-1">{formatPrice(crypto.current_price)}</div>
            <div className="text-xs flex items-center">{formatChange(crypto.price_change_percentage_24h)}</div>
          </button>
        ))}
      </div>

      {selectedCrypto && (
        <div className="space-y-6">
          <div className="p-6 bg-gray-900/50 rounded-xl border border-gray-700">
            <div className="flex items-center gap-4 mb-4">
              <img
                src={selectedCrypto.image}
                alt={selectedCrypto.name}
                className="w-12 h-12 rounded-full ring-4 ring-gray-700"
              />
              <div>
                <h4 className="text-3xl font-bold text-white tracking-tight">{selectedCrypto.name}</h4>
                <span className="text-gray-400 uppercase text-sm font-medium">{selectedCrypto.symbol}</span>
              </div>
            </div>
            <div className="flex items-baseline gap-4 flex-wrap">
              <span className="text-4xl font-bold text-gray-200">{formatPrice(selectedCrypto.current_price)}</span>
              <span className="text-xl">{formatChange(selectedCrypto.price_change_percentage_24h)}</span>
            </div>
          </div>

          <div className="p-4 bg-gray-900/50 rounded-xl border border-gray-700">
            <ResponsiveContainer width="100%" height={320}>
              <LineChart data={prepareChartData()}>
                <CartesianGrid strokeDasharray="3 3" stroke="rgba(75, 85, 99, 0.3)" />
                <XAxis dataKey="time" stroke="#9ca3af" style={{ fontSize: '12px' }} />
                <YAxis
                  domain={['auto', 'auto']}
                  tickFormatter={(value) => `$${value.toFixed(0)}`}
                  stroke="#9ca3af"
                  style={{ fontSize: '12px' }}
                />
                <Tooltip
                  formatter={(value) => formatPrice(value)}
                  contentStyle={{
                    backgroundColor: 'rgba(31, 41, 55, 0.95)',
                    border: '1px solid rgba(75, 85, 99, 0.5)',
                    borderRadius: '12px',
                    color: '#fff',
                  }}
                />
                <Legend />
                <Line
                  type="monotone"
                  dataKey="price"
                  stroke="rgb(156, 163, 175)"
                  strokeWidth={3}
                  dot={false}
                  name="Preço (USD)"
                />
              </LineChart>
            </ResponsiveContainer>
          </div>

          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            <div className="p-4 bg-gray-900/50 rounded-xl border border-gray-700 hover:border-gray-600 transition-all group">
              <div className="text-sm text-gray-400 mb-1 font-medium">Market Cap</div>
              <div className="text-lg font-bold text-white group-hover:text-gray-300 transition-colors">
                ${(selectedCrypto.market_cap / 1e9).toFixed(2)}B
              </div>
            </div>
            <div className="p-4 bg-gray-900/50 rounded-xl border border-gray-700 hover:border-gray-600 transition-all group">
              <div className="text-sm text-gray-400 mb-1 font-medium">Volume 24h</div>
              <div className="text-lg font-bold text-white group-hover:text-gray-300 transition-colors">
                ${(selectedCrypto.total_volume / 1e9).toFixed(2)}B
              </div>
            </div>
            <div className="p-4 bg-gray-900/50 rounded-xl border border-gray-700 hover:border-green-600 transition-all group">
              <div className="text-sm text-gray-400 mb-1 font-medium">Alta 24h</div>
              <div className="text-lg font-bold text-green-400 group-hover:scale-105 transition-transform">
                {formatPrice(selectedCrypto.high_24h)}
              </div>
            </div>
            <div className="p-4 bg-gray-900/50 rounded-xl border border-gray-700 hover:border-red-600 transition-all group">
              <div className="text-sm text-gray-400 mb-1 font-medium">Baixa 24h</div>
              <div className="text-lg font-bold text-red-400 group-hover:scale-105 transition-transform">
                {formatPrice(selectedCrypto.low_24h)}
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}