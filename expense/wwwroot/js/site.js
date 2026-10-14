async function deleteRecord(id, button) {
    const record = document.getElementById(`record-${id}`);

    if (!record) return;
    if (!confirm("Ready to delete?")) return;

    // 取得防偽權杖
    const token = document.querySelector(
        'input[name="__RequestVerificationToken"]'
    ).value;

    try {
        button.disabled = true;

        // 發送 AJAX 刪除請求
        const response = await fetch(`/Home/Delete/${id}`, {
            method: "POST",
            headers: {
                "RequestVerificationToken": token
            }
        });

        // 後端回傳錯誤時，不移除畫面上的紀錄
        if (!response.ok) {
            throw new Error(`Delete failed: ${response.status}`);
        }

        const result = await response.json();

        if (!result.success) {
            throw new Error("Delete was unsuccessful");
        }

        // 刪除成功後，才移除畫面上的紀錄
        record.remove();

        // 如果已經沒有紀錄，就顯示空白提示
        const remainingRecords = document.querySelectorAll(
            "#recordList .record"
        );

        if (remainingRecords.length === 0) {
            document.getElementById("emptyMessage").style.display = "";
        }
    } catch (error) {
        console.error("Delete error:", error);
        alert("刪除失敗，請稍後再試。");
    } finally {
        // 不管成功或失敗，都恢復按鈕狀態
        button.disabled = false;
    }
}
