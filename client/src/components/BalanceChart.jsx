import { Chart as ChartJS, ArcElement, Tooltip, Legend } from "chart.js";
import { Doughnut } from "react-chartjs-2";
import { formatCurrency } from "../utils/helpers";

ChartJS.register(ArcElement, Tooltip, Legend);

function BalanceChart({ balances }) {
  const positiveBalances = balances.filter((b) => b.balance > 0);
  const negativeBalances = balances.filter((b) => b.balance < 0);

  const data = {
    labels: balances.map((b) => b.userName),
    datasets: [
      {
        label: "Balance",
        data: balances.map((b) => Math.abs(b.balance)),
        backgroundColor: balances.map((b) =>
          b.balance > 0 ? "rgba(34, 197, 94, 0.8)" : "rgba(239, 68, 68, 0.8)",
        ),
        borderColor: balances.map((b) =>
          b.balance > 0 ? "rgba(34, 197, 94, 1)" : "rgba(239, 68, 68, 1)",
        ),
        borderWidth: 1,
      },
    ],
  };

  const options = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: "right",
      },
      tooltip: {
        callbacks: {
          label: function (context) {
            const label = context.label || "";
            const value = context.parsed || 0;
            const balance = balances[context.dataIndex];
            const status = balance.balance > 0 ? "gets back" : "owes";
            return `${label}: ${status} ${formatCurrency(value)}`;
          },
        },
      },
    },
  };

  if (balances.length === 0) {
    return (
      <div className="flex items-center justify-center h-64 text-gray-500">
        No balance data available
      </div>
    );
  }

  return (
    <div className="h-64">
      <Doughnut data={data} options={options} />
    </div>
  );
}

export default BalanceChart;
