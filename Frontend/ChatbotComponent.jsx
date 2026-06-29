import React, { useState, useEffect } from 'react';
import axios from 'axios';

const ChatbotComponent = () => {
  const [query, setQuery] = useState('');
  const [responses, setResponses] = useState([]);
  const [loading, setLoading] = useState(false);
  const [companies, setCompanies] = useState([]);
  const [selectedCompany, setSelectedCompany] = useState(null);
  const [token, setToken] = useState(localStorage.getItem('token'));

  const API_BASE_URL = 'http://localhost:5000/api';

  useEffect(() => {
    loadCompanies();
  }, []);

  const loadCompanies = async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}/chatbot/companies`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      setCompanies(response.data.data);
    } catch (error) {
      console.error('Error loading companies:', error);
    }
  };

  const handleSendQuery = async (e) => {
    e.preventDefault();
    if (!query.trim()) return;

    setLoading(true);
    try {
      const response = await axios.post(
        `${API_BASE_URL}/chatbot/query`,
        {
          query: query,
          companyId: selectedCompany
        },
        {
          headers: { Authorization: `Bearer ${token}` }
        }
      );

      const chatResponse = response.data.data;
      setResponses(prev => [...prev, {
        userQuery: chatResponse.query,
        response: chatResponse.response,
        generatedSql: chatResponse.generatedSql,
        data: chatResponse.data,
        isSuccessful: chatResponse.isSuccessful
      }]);

      setQuery('');
    } catch (error) {
      console.error('Error sending query:', error);
      setResponses(prev => [...prev, {
        userQuery: query,
        response: 'Error processing query',
        isSuccessful: false
      }]);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="chatbot-container" style={{ maxWidth: '1000px', margin: '0 auto', padding: '20px' }}>
      <h1>Financial Data Chatbot</h1>

      <div style={{ marginBottom: '20px' }}>
        <label>Select Company (Optional): </label>
        <select value={selectedCompany || ''} onChange={(e) => setSelectedCompany(e.target.value ? parseInt(e.target.value) : null)}>
          <option value="">All Companies</option>
          {companies.map(company => (
            <option key={company.id} value={company.id}>{company.name}</option>
          ))}
        </select>
      </div>

      <form onSubmit={handleSendQuery} style={{ marginBottom: '20px', display: 'flex', gap: '10px' }}>
        <input
          type="text"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          placeholder="Ask about turnover, quarterly data, companies, etc..."
          style={{ flex: 1, padding: '10px', fontSize: '16px' }}
          disabled={loading}
        />
        <button type="submit" disabled={loading} style={{ padding: '10px 20px', cursor: 'pointer' }}>
          {loading ? 'Loading...' : 'Send'}
        </button>
      </form>

      <div className="responses">
        {responses.map((resp, idx) => (
          <div key={idx} style={{
            border: '1px solid #ddd',
            borderRadius: '5px',
            padding: '15px',
            marginBottom: '15px',
            backgroundColor: resp.isSuccessful ? '#f0f8ff' : '#fff0f0'
          }}>
            <h3>Query: {resp.userQuery}</h3>
            <p><strong>Response:</strong> {resp.response}</p>
            <details>
              <summary>SQL Query</summary>
              <pre style={{ backgroundColor: '#f5f5f5', padding: '10px', overflow: 'auto' }}>
                {resp.generatedSql}
              </pre>
            </details>
            {resp.data && (
              <details>
                <summary>Data</summary>
                <pre style={{ backgroundColor: '#f5f5f5', padding: '10px', overflow: 'auto' }}>
                  {JSON.stringify(resp.data, null, 2)}
                </pre>
              </details>
            )}
          </div>
        ))}
      </div>
    </div>
  );
};

export default ChatbotComponent;
