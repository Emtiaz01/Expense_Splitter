import jsPDF from "jspdf";
import "jspdf-autotable";
import { formatCurrency, formatDate } from "./helpers";

/**
 * Export group expenses and settlements to PDF
 */
export const exportGroupToPDF = (
  groupData,
  expenses,
  settlements,
  balances,
) => {
  const doc = new jsPDF();

  // Title
  doc.setFontSize(20);
  doc.text(groupData.groupName, 14, 22);

  // Date
  doc.setFontSize(10);
  doc.text(`Generated: ${formatDate(new Date())}`, 14, 30);

  // Members Section
  doc.setFontSize(14);
  doc.text("Group Members", 14, 42);

  const memberData = groupData.members.map((member) => [
    member.name,
    member.role,
  ]);

  doc.autoTable({
    startY: 46,
    head: [["Member", "Role"]],
    body: memberData,
    theme: "grid",
    headStyles: { fillColor: [14, 165, 233] },
  });

  // Expenses Section
  const expenseStartY = doc.lastAutoTable.finalY + 10;
  doc.setFontSize(14);
  doc.text("Expenses", 14, expenseStartY);

  const expenseData = expenses.map((expense) => [
    formatDate(expense.createdAt),
    expense.description,
    expense.paidByUserName,
    formatCurrency(expense.amount),
    expense.splitType,
  ]);

  doc.autoTable({
    startY: expenseStartY + 4,
    head: [["Date", "Description", "Paid By", "Amount", "Split Type"]],
    body: expenseData,
    theme: "grid",
    headStyles: { fillColor: [14, 165, 233] },
  });

  // Balances Section
  const balanceStartY = doc.lastAutoTable.finalY + 10;
  doc.setFontSize(14);
  doc.text("Balances", 14, balanceStartY);

  const balanceData = balances.map((balance) => [
    balance.userName,
    formatCurrency(balance.totalPaid),
    formatCurrency(balance.totalShare),
    formatCurrency(balance.balance),
    balance.balance > 0 ? "Gets back" : "Owes",
  ]);

  doc.autoTable({
    startY: balanceStartY + 4,
    head: [["Member", "Total Paid", "Total Share", "Balance", "Status"]],
    body: balanceData,
    theme: "grid",
    headStyles: { fillColor: [14, 165, 233] },
  });

  // Settlements Section
  const settlementStartY = doc.lastAutoTable.finalY + 10;
  doc.setFontSize(14);
  doc.text("Settlements", 14, settlementStartY);

  const settlementData = settlements.map((settlement) => [
    settlement.fromName,
    settlement.toName,
    formatCurrency(settlement.amount),
  ]);

  doc.autoTable({
    startY: settlementStartY + 4,
    head: [["From", "To", "Amount"]],
    body: settlementData,
    theme: "grid",
    headStyles: { fillColor: [14, 165, 233] },
  });

  // Total Summary
  const totalExpenses = expenses.reduce(
    (sum, exp) => sum + Number(exp.amount),
    0,
  );
  const summaryY = doc.lastAutoTable.finalY + 10;
  doc.setFontSize(12);
  doc.text(`Total Expenses: ${formatCurrency(totalExpenses)}`, 14, summaryY);

  // Save PDF
  doc.save(`${groupData.groupName.replace(/\s+/g, "_")}_${Date.now()}.pdf`);
};
