import axios from 'axios';

const COINGECKO_API = 'https://api.coingecko.com/api/v3';

export const getTopCryptos = async () => {
  const response = await axios.get(`${COINGECKO_API}/coins/markets`, {
    params: {
      vs_currency: 'usd',
      order: 'market_cap_desc',
      per_page: 10,
      page: 1,
      sparkline: true,
      price_change_percentage: '24h'
    }
  });
  return response.data;
};

export const getCurrentPrices = async (symbols) => {
  const ids = symbols.map(s => getCoinGeckoId(s)).join(',');
  const response = await axios.get(`${COINGECKO_API}/simple/price`, {
    params: {
      ids,
      vs_currencies: 'usd',
      include_24hr_change: true
    }
  });
  return response.data;
};

const getCoinGeckoId = (symbol) => {
  const map = {
    'BTC': 'bitcoin',
    'ETH': 'ethereum',
    'BNB': 'binancecoin',
    'SOL': 'solana',
    'ADA': 'cardano',
    'XRP': 'ripple',
    'DOT': 'polkadot',
    'DOGE': 'dogecoin',
    'AVAX': 'avalanche-2',
    'MATIC': 'matic-network'
  };
  return map[symbol.toUpperCase()] || symbol.toLowerCase();
};